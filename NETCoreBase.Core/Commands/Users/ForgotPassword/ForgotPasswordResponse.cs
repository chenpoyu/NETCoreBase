namespace NETCoreBase.Core.Commands.Users
{
    public class ForgotPasswordResponse
    {
        /// <summary>
        /// 密碼重設 Token（骨架模式直接回傳，正式環境應透過 Email 傳送）
        /// </summary>
        public string ResetToken { get; set; }
    }
}
