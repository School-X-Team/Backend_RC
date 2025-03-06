namespace Backend_RC.Services;

public interface ICardNumberGenerator
{
    /// <summary>
    /// Генерирует уникальный номер виртуальной карты
    /// </summary>
    /// <returns></returns>
    string GenerateCardNumber();
}
public class CardNumberGenerator : ICardNumberGenerator
{
    private static readonly Random _random = new();

    public string GenerateCardNumber()
    {
        string cardNumber = string.Concat(Enumerable.Range(0, 15).Select(_ => _random.Next(0, 10).ToString()));

        //вычисляю контрольную цифру Luhn и добавляю ее
        string fullCardNumber = cardNumber + GetLuhnCheckDigit(cardNumber);

        return fullCardNumber;
    }
    /// <summary>
    /// Вычисление контрольной цифры с помощью алгоритма Луна
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    private static int GetLuhnCheckDigit(string number)
    {
        int sum = 0;
        bool alternate = true;

        for (int i = number.Length - 1; i >= 0; i--)
        {
            int digit = number[i] - '0';
            if (alternate)
            {
                digit *= 2;
                if (digit > 0) digit -= 9;
            }
            sum += digit;
            alternate = !alternate;
        }

        return (10 - (sum % 10)) % 10;
    }
}
