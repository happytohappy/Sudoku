using UnityEngine;

public partial class UIWindowMultiGame : UIWindowBase
{
    private int[,] m_SolvedTempGrid = new int[9, 9];                                      
    private int[,] m_RiddleTempGrid = new int[9, 9];                                     
    private int[] m_SolvedGrid = new int[81];
    private int[] m_RiddleGrid = new int[81];
    private int piecesToErase = 50;                                               

    private void CreateInit()
    {
        InitGrid(ref m_SolvedTempGrid);
        ShuffleGrid(ref m_SolvedTempGrid, 10);
        CreateRiddleGrid();
    }

    private void InitGrid(ref int[,] grid)                                          // 9*9 배열 값을 생성하는 함수
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                grid[i, j] = (i * 3 + i / 3 + j) % 9 + 1;
            }
        }

        int n1 = 8 * 3;
        int n2 = 8 / 3;
        int n = (n1 + n2 + 0) % 9 + 1;
    }

    private void ShuffleGrid(ref int[,] grid, int shuffleAmount)                    // 일정한 패턴의 grid를 불규칙적으로 섞는 함수
    {
        for (int i = 0; i < shuffleAmount; i++)
        {
            int value1 = Random.Range(1, 10);
            int value2 = Random.Range(1, 10);

            MixTwoGridCells(ref grid, value1, value2);
        }
    }

    private void MixTwoGridCells(ref int[,] grid, int value1, int value2)           // 두개의 셀의 값을 확인하고 서로 교환하는 함수
    {
        int x1 = 0;
        int x2 = 0;
        int y1 = 0;
        int y2 = 0;

        for (int i = 0; i < 9; i += 3)
        {
            for (int j = 0; j < 9; j += 3)
            {
                for (int k = 0; k < 3; k++)
                {
                    for (int l = 0; l < 3; l++)
                    {
                        if (grid[i + k, j + l] == value1)
                        {
                            x1 = i + k;
                            y1 = j + l;
                        }

                        if (grid[i + k, j + l] == value2)
                        {
                            x2 = i + k;
                            y2 = j + l;
                        }
                    }
                }

                grid[x1, y1] = value2;
                grid[x2, y2] = value1;
            }
        }
    }

    private void CreateRiddleGrid()
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                m_RiddleTempGrid[i, j] = m_SolvedTempGrid[i, j];
            }
        }

        for (int i = 0; i < piecesToErase; i++)
        {
            int x1 = Random.Range(0, 9);
            int y1 = Random.Range(0, 9);

            while (m_RiddleTempGrid[x1, y1] == 0)
            {
                x1 = Random.Range(0, 9);
                y1 = Random.Range(0, 9);
            }

            m_RiddleTempGrid[x1, y1] = 0;
        }

        int index = 0;
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                m_SolvedGrid[index] = m_SolvedTempGrid[i, j];
                m_RiddleGrid[index] = m_RiddleTempGrid[i, j];
                index++;
            }
        }
    }
}