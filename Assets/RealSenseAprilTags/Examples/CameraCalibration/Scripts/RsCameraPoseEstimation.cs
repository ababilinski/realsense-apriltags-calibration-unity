using Babilinapps.RealSenseAprilTags.Utils.Math;
using Intel.RealSense;
using System.Collections;
using System.Collections.Generic;
using Babilinapps.RealSenseAprilTags.Utils;
using UnityEngine;

namespace Babilinapps.RealSenseAprilTags.Examples
{
    public class RsCameraPoseEstimation : MonoBehaviour
    {

        [Header("Frame Providers")]
        public RsStreamTextureRenderer RgbStreamTextureRenderer;

        public RsPointCloudRenderer RsPointCloud;


        [Header("Tracking")]
        [SerializeField] int _decimation = 4;

        [SerializeField] private float _decodeSharpening = 4;
        [SerializeField] private float _quadSigma = .8f;
        [SerializeField] private int _refineEdges = 1;
        [SerializeField] float _tagSize = 0.05f;
        public Transform originTagPoint;
        public bool MoveCamera = true;


        [Header("Debug")]
        public Color tagOverlayColor = Color.yellow;

        [SerializeField] private bool _drawWithPointCloudOffset = true; //Intel RealSense does not allow you to align depth + color on the raw data when creating a point cloud
        [SerializeField] private Transform _debugCameraTransform;

        public int CalibrationTicks; // Increases only when at least one marker is found
        public bool IsStreaming { get; private set; }
        private Texture2D _rgbTexture;
        private Intrinsics _intrinsics;
        private int _cameraFps;
        private Color32[] _readBuffer;
        private Material _tagDebugMaterial = null;

        // AprilTag detector and drawer
        private AprilTag.TagDetector _detector;
        private TagDrawer _drawer;

        //used to avoid fatal crash
        private int _cachedDecimation;
        private float _cachedDecodeSharpening;
        private float _cachedQuadSigma;
        private int _cachedRefineEdges;
        private float _cachedTagSize;
        private bool _wasReset;

        private void Awake()
        {

            Material pointCloudMaterial = new Material(Shader.Find("Custom/PointCloudGeom"));
            _tagDebugMaterial = new Material(Shader.Find("Unlit/Color")) {color = tagOverlayColor};

            RsPointCloud.GetComponentInChildren<MeshRenderer>(true).material = pointCloudMaterial;
            _debugCameraTransform.GetComponentInChildren<MeshRenderer>(true).material = _tagDebugMaterial;

            RgbStreamTextureRenderer.textureBinding.AddListener((texture) =>
                                                                {
                                                                    pointCloudMaterial.mainTexture = texture;
                                                                    _rgbTexture = texture as Texture2D;
                                                                    IsStreaming = true;
                                                                });


        }

        IEnumerator Start()
        {
            _cachedDecimation = _decimation;
            _cachedDecodeSharpening = _decodeSharpening;
            _cachedQuadSigma = _quadSigma;
            _cachedRefineEdges = _refineEdges;
            _cachedTagSize = _tagSize;
            yield return new WaitUntil(() => RgbStreamTextureRenderer.Source && RgbStreamTextureRenderer.Source.Streaming);
            using (var profile = RgbStreamTextureRenderer.Source.ActiveProfile.GetStream<VideoStreamProfile>(Stream.Color))
            {
                _cameraFps = profile.Framerate;
                Init(profile.GetIntrinsics());
            }



        }



        public void Init(Intrinsics intrinsics)
        {
            _intrinsics = intrinsics;

            _readBuffer = new Color32[intrinsics.width * intrinsics.height];

            _drawer = new TagDrawer(_tagDebugMaterial);
            _detector = new AprilTag.TagDetector(_intrinsics.width, _intrinsics.height, _cachedDecimation, _cachedDecodeSharpening, _cachedQuadSigma, _cachedRefineEdges);



        }

        private void OnDestroy()
        {
            Destroy(_rgbTexture);
            _detector?.Dispose();
            _drawer?.Dispose();
        }

        private void CalculateAveragePose()
        {
            int counter = 0;
            var positions = new List<Vector3>();
            var rotations = new List<Quaternion>();


            foreach (var pose in _detector.DetectedTags)
            {

                positions.Add(pose.Position);
                rotations.Add(pose.Rotation);
                Vector3 initialPosition = pose.Position;
                Quaternion initialRotation = pose.Rotation;

                if (_drawWithPointCloudOffset)
                {
                    initialPosition += Vector3.right * .06f;
                }

                initialPosition = RsPointCloud.transform.TransformPoint(initialPosition);

                _drawer.Draw(pose.ID, initialPosition, initialRotation, _tagSize);


                counter++;
            }

            if (counter > 0 && MoveCamera)
            {
                CalibrationTicks++;
                var averageRotation = MathUtils.Average(rotations.ToArray());
                Vector3 result = MathUtils.Average(positions.ToArray());
                var averagePosition = result / positions.Count;
                var poseMatrix = CalibrationUtils.ConvertToMatrix(averagePosition, averageRotation);

                poseMatrix = originTagPoint.transform.localToWorldMatrix * poseMatrix.inverse;
                var translation = MathUtils.ExtractTranslationFromMatrix(ref poseMatrix);
                var rotation = poseMatrix.rotation;

                RsPointCloud.transform.SetPositionAndRotation(translation, rotation);
                _debugCameraTransform.SetPositionAndRotation(translation, rotation);


            }
        }

        bool NeedsReset()
        {

            if (_wasReset)
            {
                _detector = new AprilTag.TagDetector(_intrinsics.width, _intrinsics.height, _cachedDecimation, _cachedDecodeSharpening, _cachedQuadSigma, _cachedRefineEdges);
                _wasReset = false;
                return true;

            }

            if (_cachedDecimation != _decimation || _cachedDecodeSharpening != _decodeSharpening || _cachedQuadSigma != _quadSigma || _cachedRefineEdges != _refineEdges || _cachedTagSize != _tagSize)
            {
                _cachedDecimation = _decimation;
                _cachedDecodeSharpening = _decodeSharpening;
                _cachedQuadSigma = _quadSigma;
                _cachedRefineEdges = _refineEdges;
                _cachedTagSize = _tagSize;
                _detector.Dispose();
                _wasReset = true;
                return true;
            }


            return false;
        }

        private void Update()
        {
            if (!IsStreaming || NeedsReset())
            {
                return;
            }

            if (!MoveCamera && CalibrationTicks > 0)
            {
                CalibrationTicks = 0;
            }

            if (Time.frameCount % _cameraFps == 0)
            {
                _readBuffer = _rgbTexture.GetPixels32();
                _detector.ProcessFlippedImage(_readBuffer, _intrinsics.fx, _intrinsics.fy, _intrinsics.ppx, _intrinsics.ppy, _tagSize);
            }


            CalculateAveragePose();



        }



    }
}