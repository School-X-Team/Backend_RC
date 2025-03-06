﻿namespace Backend_RC.Models;

public class IndicatedCardModel
{
    public Guid Id { get; set; }
    public string CardNumber { get; set; }
    public bool isConfirmed { get; set; } = false;
    public string UserId { get; set; }
    public DateTime LinkedAt { get; set; }
    public User User { get; set; }
}
