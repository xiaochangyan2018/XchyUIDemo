using System.Text.RegularExpressions;

namespace XcyUI.Controls.Utils
{
    /// <summary>
    /// 输入框验证正则帮助类
    /// 对应 InputType 枚举
    /// </summary>
    public static class InputRegex
    {
        #region 常用正则常量

        /// <summary>
        /// 普通文本（无特殊限制，防注入）
        /// </summary>
        public const string Text = @"^[^<>""']{0,500}$";

        /// <summary>
        /// 纯数字（整数）
        /// </summary>
        public const string Number = @"^-?\d+$";

        /// <summary>
        /// 小数（支持正负、小数点后最多8位，适合金额）
        /// </summary>
        public const string Decimal = @"^-?(\d+(\.\d*)?|\.\d+)$";

        /// <summary>
        /// 中国大陆手机号
        /// </summary>
        public const string Phone = @"^1[3-9]\d{9}$";

        /// <summary>
        /// 固定电话（带区号）
        /// </summary>
        public const string Tel = @"^(0\d{2,3}-?)?\d{7,8}$";

        /// <summary>
        /// 邮箱
        /// </summary>
        public const string Email = @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";

        /// <summary>
        /// URL 地址
        /// </summary>
        public const string Url = @"^(https?|ftp)://[^\s]+$";

        /// <summary>
        /// 身份证（15位 / 18位）
        /// </summary>
        public const string IdCard = @"^\d{15}|\d{18}|\d{17}[\dXx]$";

        /// <summary>
        /// 普通密码（6-20位）
        /// </summary>
        public const string Password = @"^.{6,20}$";

        /// <summary>
        /// 强密码（必须包含：大小写+数字+特殊字符，8-20位）
        /// </summary>
        public const string StrongPassword = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,20}$";

        /// <summary>
        /// 验证码（4-8位字母数字）
        /// </summary>
        public const string VerifyCode = @"^[a-zA-Z0-9]{4,8}$";

        /// <summary>
        /// 邮政编码
        /// </summary>
        public const string ZipCode = @"^\d{6}$";

        /// <summary>
        /// 中国大陆车牌
        /// </summary>
        public const string LicensePlate = @"^[京津沪渝冀豫云辽黑湘皖鲁新苏浙赣鄂桂甘晋蒙陕吉闽贵粤青藏川宁琼][A-Z][A-Z0-9]{5,6}$";

        /// <summary>
        /// 统一社会信用代码
        /// </summary>
        public const string UnifiedSocialCreditCode = @"^[0-9A-HJ-NPQRTUWXY]{18}$";

        /// <summary>
        /// 银行卡号（16-22位纯数字）
        /// </summary>
        public const string BankCard = @"^\d{16,22}$";

        /// <summary>
        /// 仅中文
        /// </summary>
        public const string Chinese = @"^[\u4e00-\u9fa5]+$";

        /// <summary>
        /// 仅英文
        /// </summary>
        public const string English = @"^[a-zA-Z]+$";

        /// <summary>
        /// 英文 + 数字
        /// </summary>
        public const string EnglishAndNumber = @"^[a-zA-Z0-9]+$";

        /// <summary>
        /// IP 地址
        /// </summary>
        public const string IpAddress = @"^((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$";

        /// <summary>
        /// 日期 yyyy-MM-dd
        /// </summary>
        public const string Date = @"^\d{4}-\d{2}-\d{2}$";

        /// <summary>
        /// 时间 HH:mm:ss
        /// </summary>
        public const string Time = @"^\d{2}:\d{2}:\d{2}$";

        /// <summary>
        /// 日期时间 yyyy-MM-dd HH:mm:ss
        /// </summary>
        public const string DateTime = @"^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}$";

        #endregion

        #region 通用验证方法

        /// <summary>
        /// 根据输入类型验证内容
        /// </summary>
        public static bool Validate(InputType type, string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            string pattern = Text;
            switch (type)
            {
                case InputType.Text:
                    pattern = Text;
                    break;
                case InputType.Number:
                    pattern = Number;
                    break;
                case InputType.Decimal:
                    pattern = Decimal;
                    break;
                case InputType.Phone:
                    pattern = Phone;
                    break;
                case InputType.Tel:
                    pattern = Tel;
                    break;
                case InputType.Email:
                    pattern = Email;
                    break;
                case InputType.Url:
                    pattern = Url;
                    break;
                case InputType.IdCard:
                    pattern = IdCard;
                    break;
                case InputType.VerifyCode:
                    pattern = VerifyCode;
                    break;
                case InputType.ZipCode:
                    pattern = ZipCode;
                    break;
                case InputType.LicensePlate:
                    pattern = LicensePlate;
                    break;
                case InputType.UnifiedSocialCreditCode:
                    pattern = UnifiedSocialCreditCode;
                    break;
                case InputType.BankCard:
                    pattern = BankCard;
                    break;
                case InputType.Chinese:
                    pattern = Chinese;
                    break;
                case InputType.English:
                    pattern = English;
                    break;
                case InputType.EnglishAndNumber:
                    pattern = EnglishAndNumber;
                    break;
                case InputType.IpAddress:
                    pattern = IpAddress;
                    break;
                case InputType.Date:
                    pattern = Date;
                    break;
                case InputType.Time:
                    pattern = Time;
                    break;
                case InputType.DateTime:
                    pattern = DateTime;
                    break;
            }
            return Regex.IsMatch(value, pattern);
        }

        #endregion
    }
}
