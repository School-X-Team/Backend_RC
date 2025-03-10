namespace Backend_RC.DTO;

public class ChangePasswordModel
{
    /// <summary>
    /// Старый пароль пользователя
    /// </summary>
    public string OldPassword { get; set; }
    /// <summary>
    /// Новый пароль
    /// </summary>
    public string NewPassword { get; set; }
    /// <summary>
    /// Подтверждение нового пароля
    /// </summary>
    public string ConfirmPassword { get; set; }
}
