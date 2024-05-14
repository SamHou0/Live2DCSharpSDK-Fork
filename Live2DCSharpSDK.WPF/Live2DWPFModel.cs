using Live2DCSharpSDK.App;
using OpenTK.Mathematics;
using OpenTK.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using OpenTK.Graphics.OpenGL4;
using System.IO;
using Live2DCSharpSDK.Framework.Motion;
using Live2DCSharpSDK.Framework;
using static Live2DCSharpSDK.Framework.ModelSettingObj.FileReference;
using System.Windows.Controls;
using System.Xml.Linq;

namespace Live2DCSharpSDK.WPF
{
    /// <summary>
    /// Live2DWPF模型控件
    /// </summary>
    public class Live2DWPFModel
    {
        /// <summary>
        /// 模型名字
        /// </summary>
        public string Name;
        /// <summary>
        /// 当前动画是否正在播放
        /// </summary>
        public bool IsPlaying { get; set; } = false;
        /// <summary>
        /// 附着于的窗体控件
        /// </summary>
        public GLWpfControl GLControl { get; private set; }
        /// <summary>
        /// L2D管理器
        /// </summary>
        public LAppDelegate LAPP { get; private set; }
        /// <summary>
        /// L2D模型
        /// </summary>
        public LAppModel LModel { get; private set; }
        /// <summary>
        /// 新建一个Live2D模型
        /// </summary>
        /// <param name="Path">Live2D模型位置 (moc3)</param>
        public Live2DWPFModel(string Path)
        {
            var file = new FileInfo(Path);
            if (!file.Exists || file.DirectoryName == null)
            {
                throw new FileNotFoundException();
            }
            GLControl = new GLWpfControl();
            GLControl.SizeChanged += GLControl_Resized;
            var settings = new GLWpfControlSettings
            {
                MajorVersion = 3,
                MinorVersion = 3
            };
            GLControl.Start(settings);
            LAPP = new(new OpenTKWPFApi(GLControl), Console.WriteLine)
            {
                BGColor = new(0, 0, 0, 0)
            };
            Name = file.Name[..^file.Extension.Length];
            LModel = LAPP.Live2dManager.LoadModel(file.DirectoryName, file.Name[..^file.Extension.Length]);
        }
        /// <summary>
        /// 开始播放/渲染
        /// </summary>
        public void Start()
        {
            IsPlaying = true;
            GLControl.Render += GLControl_Render;
        }
        /// <summary>
        /// 停止播放/渲染
        /// </summary>
        public void Stop()
        {
            IsPlaying = false;
            GLControl.Render -= GLControl_Render;
        }
        private void GLControl_Render(TimeSpan obj)
        {
            GL.ClearColor(Color4.Blue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            LAPP.Run((float)obj.TotalSeconds);
        }
        private void GLControl_Resized(object sender, SizeChangedEventArgs e)
        {
            if (LAPP == null || (int)GLControl.ActualWidth == 0 || (int)GLControl.ActualHeight == 0 || !IsPlaying)
                return;
            LAPP.Resize();
            GL.Viewport(0, 0, (int)GLControl.ActualWidth, (int)GLControl.ActualHeight);
        }
        /// <summary>
        /// 播放动作文件
        /// </summary>
        /// <param name="MotionPath">动画文件地址(motion3.json)</param>
        /// <param name="priority">优先级</param>
        /// <param name="onFinishedMotionHandler">回调</param>
        /// <returns></returns>
        public CubismMotionQueueEntry? StartMotion(string MotionPath, MotionPriority priority = MotionPriority.PriorityNormal, string MothionName = "", FinishedMotionCallback? onFinishedMotionHandler = null)
        {
            if (priority == MotionPriority.PriorityForce)
            {
                LModel._motionManager.ReservePriority = priority;
            }
            else if (!File.Exists(MotionPath))
            {
                return null;
            }
            //判断优先级是否允许播放
            else if (!LModel._motionManager.ReserveMotion(priority))
            {
                CubismLog.Debug("[Live2D]can't start motion.");
                return null;
            }
            if (string.IsNullOrEmpty(MothionName))
            {
                MothionName = Path.GetFileNameWithoutExtension(MotionPath);
            }

            CubismMotion motion;
            if (!LModel._motions.TryGetValue(MothionName, out var value))
            {
                motion = new CubismMotion(MotionPath, onFinishedMotionHandler);
                //float fadeTime = item.FadeInTime;
                //if (fadeTime >= 0.0f)
                //{
                //    motion.FadeInSeconds = fadeTime;
                //}

                //fadeTime = item.FadeOutTime;
                //if (fadeTime >= 0.0f)
                //{
                //    motion.FadeOutSeconds = fadeTime;
                //}
                motion.SetEffectIds(LModel._eyeBlinkIds, LModel._lipSyncIds);
            }
            else
            {
                motion = (value as CubismMotion)!;
                motion.OnFinishedMotion = onFinishedMotionHandler;
            }

            CubismLog.Debug($"[Live2D]start motion: [{MotionPath}]");
            return LModel._motionManager.StartMotionPriority(motion, priority);
        }

    }
}
