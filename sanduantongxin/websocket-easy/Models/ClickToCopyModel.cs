using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace websocket_easy.Models
{
    /// <summary>
    /// 点抄
    /// </summary>
    public class ClickToCopyModel
    {
        /// <summary>
        /// 集中器编号
        /// </summary>
        public string ConcentratorNo { get; set; }


        /// <summary>
        /// 水表地址
        /// </summary>
        public string WaterMtrAddress { get; set; }


        /// <summary>
        /// 累计流量
        /// </summary>
        public double AccumVal { get; set; }

        /// <summary>
        /// 是否已发送指令
        /// </summary>
        public bool IsDownCommand { get; set; }
    }

}