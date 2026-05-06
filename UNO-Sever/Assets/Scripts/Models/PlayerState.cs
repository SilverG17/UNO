using System;
using System.Collections.Generic;

[Serializable]
public class PlayerState
{
    public string PlayerId;
    public List<Card> Hand = new();
}