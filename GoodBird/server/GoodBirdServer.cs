using System;

namespace GoodBird
{
    /// <summary>
    /// 咕咕机消息发送服务器
    /// </summary>
    public class GoodBirdServer
    {

        // 将图片转为base64位图(压缩)的接口地址
        private string GetSignalBase64Pic = "http://open.memobird.cn/home/getSignalBase64Pic";
        // 打印小纸条的接口地址
        private string PrintPaper = "http://open.memobird.cn/home/printpaper";
        // 用户关联的接口地址
        private string SetUserBind = "http://open.memobird.cn/home/setuserbind";

        private HttpHelper GetSignalBase64PicHttp;
        private HttpHelper PrintPaperHttp;
        private HttpHelper SetUserBindHttp;
        // 每日刷新时间的计时器
        private TimeTick RefreshTimeTick;
        // 每分钟确认时间匹配的计时器
        private TimeTick ConfirmTimeTick;
        public GoodBirdServer()
        {
            GetSignalBase64PicHttp = new HttpHelper(GetSignalBase64Pic);
            PrintPaperHttp = new HttpHelper(PrintPaper);
            SetUserBindHttp = new HttpHelper(SetUserBind);
        }

        /// <summary>
        /// 启动服务器,开启定时
        /// </summary>
        public void Start()
        {
            RefreshTimeTick = new TimeTick(Common.UpdateDay);
            Handle RefreshTimeHandle = new Handle(RefreshTime);
            RefreshTimeTick.Start(RefreshTimeHandle);

            ConfirmTimeTick = new TimeTick(Common.ConfirmTime);
            Handle ConfirmTimeHandle = new Handle(ConfirmTime);
            ConfirmTimeTick.Start(ConfirmTimeHandle);
        }
        /// <summary>
        /// 停止服务器,停止计时器
        /// </summary>
        public void Stop(){
            RefreshTimeTick.Stop();
            ConfirmTimeTick.Stop();
        }
        /// <summary>
        /// 每天刷新发送时间
        /// </summary>
        private void RefreshTime(object source, System.Timers.ElapsedEventArgs e)
        {
            int Length = Common.SendPeriods.Length;
            for (int i = 0; i < Length; i++)
                Common.SendPeriods[i].RefreshTime();
        }
        /// <summary>
        /// 检查时间,到时发送
        /// </summary>
        private void ConfirmTime(object source, System.Timers.ElapsedEventArgs e)
        {
            int Length = Common.SendPeriods.Length;
            for (int i = 0; i < Length; i++)
            {
                if (Common.SendPeriods[i].ConfirmTime())
                {
                    SendImg(Common.SendPeriods[i].ImgDic, Common.SendPeriods[i].Message);
                }
            }
        }

        /// <summary>
        /// 发送咕咕机小纸条
        /// </summary>
        /// <param name="imgPath"></param>
        private void SendImg(string imgPath, string message)
        {
            string[] fileList = FileHelper.GetFileList(imgPath);
            string filePath = imgPath + fileList[new Random().Next(fileList.Length)];
            string base64 = ImageHelper.ImageToBase64(filePath);
            string grayBase64 = GetGrayBase64Pic(base64);

            System.Collections.Generic.Dictionary<string, string> queries = new System.Collections.Generic.Dictionary<string, string>();

            string printContent = "";
            if (message != "")
                printContent += $"T:{RegexHelper.TextToBase64(message)}|";
            printContent += $"P:{grayBase64}";

            foreach (var item in Common.Birds)
            {
                queries.Clear();
                queries.Add("ak", Common.ACCESS_KEY);
                queries.Add("timestamp", DateTime.Now.ToString("yyyy-MM-ddhh:mm:ss"));
                queries.Add("memobirdID", item.MemobirdID);
                queries.Add("userID", LinkBird(item));
                queries.Add("printcontent", printContent);

                PrintPaperHttp.SetQueries(queries);
                string result = PrintPaperHttp.HttpPost();
                Console.WriteLine(result);
            }
        }
        /// <summary>
        /// 按照文档的顺序关联账号,获取userId
        /// </summary>
        /// <param name="birdInfo"></param>
        /// <returns></returns>
        private string LinkBird(MemoBirdInfo birdInfo)
        {
            System.Collections.Generic.Dictionary<string, string> queries = new System.Collections.Generic.Dictionary<string, string>();
            queries.Clear();

            queries.Add("ak", Common.ACCESS_KEY);
            queries.Add("timestamp", DateTime.Now.ToString("yyyy-MM-ddhh:mm:ss"));
            queries.Add("memobirdID", birdInfo.MemobirdID);
            queries.Add("useridentifying", birdInfo.UserID);

            SetUserBindHttp.SetQueries(queries);
            string result = SetUserBindHttp.HttpPost();
            result = RegexHelper.GetNumElem(result, "showapi_userid");
            return result;
        }

        /// <summary>
        /// 请求图片位图转换接口获取转换后的位图图片
        /// </summary>
        /// <param name="originBase64"></param>
        /// <returns></returns>
        private string GetGrayBase64Pic(string originBase64)
        {
            System.Collections.Generic.Dictionary<string, string> queries = new System.Collections.Generic.Dictionary<string, string>();
            queries.Clear();

            queries.Add("ak", Common.ACCESS_KEY);
            queries.Add("imgBase64String", originBase64);
            GetSignalBase64PicHttp.SetQueries(queries);
            string base64 = GetSignalBase64PicHttp.HttpPost();
            base64 = RegexHelper.GetElem(base64, "result");
            return base64;
        }
    }
}