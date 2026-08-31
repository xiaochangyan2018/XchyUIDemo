namespace XcyUI.Controls
{
    public enum InputType
    {
        /// <summary>
        /// 未指定 / 默认
        /// </summary>
        None = 0,

        /// <summary>
        /// 普通文本（姓名、标题、备注等）
        /// </summary>
        Text = 1,

        /// <summary>
        /// 数字（整数）
        /// </summary>
        Number = 2,

        /// <summary>
        /// 小数（金额、价格、精度值）
        /// </summary>
        Decimal = 3,

        /// <summary>
        /// 手机号码
        /// </summary>
        Phone = 4,

        /// <summary>
        /// 固定电话
        /// </summary>
        Tel = 5,

        /// <summary>
        /// 电子邮箱
        /// </summary>
        Email = 6,

        /// <summary>
        /// 网址 URL
        /// </summary>
        Url = 7,

        /// <summary>
        /// 身份证号码
        /// </summary>
        IdCard = 8,


        /// <summary>
        /// 验证码（短信/图形）
        /// </summary>
        VerifyCode = 11,

        /// <summary>
        /// 邮政编码
        /// </summary>
        ZipCode = 12,

        /// <summary>
        /// 车牌号码
        /// </summary>
        LicensePlate = 13,

        /// <summary>
        /// 统一社会信用代码
        /// </summary>
        UnifiedSocialCreditCode = 14,

        /// <summary>
        /// 银行卡号
        /// </summary>
        BankCard = 15,

        /// <summary>
        /// 汉字（仅允许中文）
        /// </summary>
        Chinese = 16,

        /// <summary>
        /// 英文（仅字母）
        /// </summary>
        English = 17,

        /// <summary>
        /// 英文 + 数字
        /// </summary>
        EnglishAndNumber = 18,

        /// <summary>
        /// IP 地址
        /// </summary>
        IpAddress = 19,

        /// <summary>
        /// 多行文本（备注、描述）
        /// </summary>
        MultilineText = 20,

        /// <summary>
        /// 搜索框
        /// </summary>
        Search = 21,

        /// <summary>
        /// 日期（yyyy-MM-dd）
        /// </summary>
        Date = 22,

        /// <summary>
        /// 时间（HH:mm:ss）
        /// </summary>
        Time = 23,

        /// <summary>
        /// 日期时间
        /// </summary>
        DateTime = 24
    }
}
