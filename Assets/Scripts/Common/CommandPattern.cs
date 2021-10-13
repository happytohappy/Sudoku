using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandPattern
{
    public int m_LastNum = 0;
    public int[] m_RiddleArr = new int[81];
    public int[,] m_MemoArr = new int[81,9];

    public virtual void ExecuteAction() { }
    public virtual void UndoAction() { }
}

public class NumberInputCommand : CommandPattern
{
    public NumberInputCommand(int LastNum, int[] riddleArr, int[,] memoArr)
    {
        m_LastNum = LastNum;
        m_RiddleArr = riddleArr;
        m_MemoArr = memoArr;
    }
}
