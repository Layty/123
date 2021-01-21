using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Toolkit.Mvvm.Input;
using 三相智慧能源网关调试软件.ViewModel;
using 三相智慧能源网关调试软件.ViewModel.DlmsViewModels;

namespace 三相智慧能源网关调试软件.Model
{
    /// <summary>
    /// DLMS曲线任务中心,定义一个定时器，以及曲线任务，通过回调方法通知关注者执行任务
    /// </summary>
    public class TaskCenterViewModel
    {
        /// <summary>
        /// 任务间隔,单位毫秒，默认5分钟即300000秒
        /// </summary>
        public int Period { get; set; } = 300000;

       // public ProfileGenericViewModel ccc { get; set; }

        public Timer Timer;


        public TaskCenterViewModel()
        {
          //  ccc = SimpleIoc.Default.GetInstance<ProfileGenericViewModel>();
            Timer = new Timer();
        }

        public void StopTask()
        {
            if (Timer.Enabled)
            {
                Timer.Stop();
            }
        }

        public void StartTask()
        {
            Timer = new Timer();
            Timer.Interval = Period;
            Timer.Enabled = true;
            Timer.Start();
        }
    }
}