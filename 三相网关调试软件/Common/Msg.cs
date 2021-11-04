using Microsoft.Toolkit.Mvvm.Messaging;
using System.ComponentModel;
using System.Threading.Tasks;

namespace 三相智慧能源网关调试软件.Common
{
    public enum Notify
    {
        /// <summary>
        /// 错误
        /// </summary>
        [Description("错误")]
        Error,
        /// <summary>
        /// 警告
        /// </summary>
        [Description("警告")]
        Warning,
        /// <summary>
        /// 提示信息
        /// </summary>
        [Description("提示信息")]
        Info,
        /// <summary>
        /// 询问信息
        /// </summary>
        [Description("询问信息")]
        Question,
    }
    /// <summary>
    /// 消息类
    /// </summary>
    public class Msg
    {
        /// <summary>
        /// 信息提示
        /// </summary>
        /// <param name="msg"></param>
        public static void Info(string msg)
        {
            WeakReferenceMessenger.Default.Send(msg, "Snackbar");
        }

        /// <summary>
        /// 错误提示
        /// </summary>
        /// <param name="msg"></param>
        public static void Error(string msg)
        {
            WeakReferenceMessenger.Default.Send(msg, "Snackbar");
        }

        /// <summary>
        /// 真香警告
        /// </summary>
        /// <param name="msg"></param>
        public static void Warning(string msg)
        {
            WeakReferenceMessenger.Default.Send(msg, "Snackbar");
        }

        /// <summary>
        /// 真香询问
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async static Task<bool> Question(string msg)
        {
            return await Show(Notify.Question, msg);
        }

        /// <summary>
        /// 弹出窗口
        /// </summary>
        /// <param name="notify">类型</param>
        /// <param name="msg">文本信息</param>
        /// <returns></returns>
        private async static Task<bool> Show(Notify notify, string msg)
        {
            string Icon = string.Empty;
            string Color = string.Empty;
            switch (notify)
            {
                case Notify.Error:
                    Icon = "CommentWarning";
                    Color = "#FF4500";
                    break;
                case Notify.Warning:
                    Icon = "CommentWarning";
                    Color = "#FF8247";
                    break;
                case Notify.Info:
                    Icon = "CommentProcessingOutline";
                    Color = "#1C86EE";
                    break;
                case Notify.Question:
                    Icon = "CommentQuestionOutline";
                    Color = "#20B2AA";
                    break;
            }
            //            var dialog = NetCoreProvider.ResolveNamed<IMsgCenter>("MsgCenter");
            //            var result = await dialog.Show(new { Msg = msg, Color, Icon });
            //            return result;

            return true;
        }
    }
}