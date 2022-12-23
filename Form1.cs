using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exam_Checkers
{
    public partial class Form1 : Form
    {

        Button cell_button;
        DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory());
        Image white_figure;
        Image black_figure;
        const int map_size = 8;
        int[,] map = new int[map_size, map_size];
        int cell_size = 80;
        int current_player;

        Button prev_button;
        Button pressed_button;
        bool isMoving;
        bool isContinue = false;
        Button[,] all_buttons = new Button[map_size, map_size];

        int countEatSteps = 0;

        List<Button> easyStep = new List<Button>();

        int score_black = 0;
        int score_white = 0;
        public Form1()
        {
            InitializeComponent();
            dir = dir.Parent.Parent;
            white_figure = new Bitmap(Image.FromFile(dir.FullName + "\\Images\\white.png"), new Size(cell_size - 8, cell_size - 8));
            black_figure = new Bitmap(Image.FromFile(dir.FullName + "\\Images\\black.png"), new Size(cell_size - 8, cell_size - 8));
            StartPosition = FormStartPosition.CenterScreen;
            FieldGame();
        }
        public void FieldGame()
        {
            current_player = 1;
            isMoving = false;
            prev_button = null;
            map = new int[map_size, map_size]
            {
                { 0,2,0,2,0,2,0,2 },
                { 2,0,2,0,2,0,2,0 },
                { 0,2,0,2,0,2,0,2 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 1,0,1,0,1,0,1,0 },
                { 0,1,0,1,0,1,0,1 },
                { 1,0,1,0,1,0,1,0 }
            };
            CreateCellOnTheMap();
        }
        public void CreateCellOnTheMap()
        {
            for (int i = 0; i < map_size; i++)
            {
                for (int j = 0; j < map_size; j++)
                {
                    cell_button = new Button();
                    cell_button.Size = new Size(cell_size, cell_size);
                    cell_button.Location = new Point(j * cell_size, i * cell_size);
                    if (i % 2 != 0 && j % 2 == 0 || i % 2 == 0 && j % 2 != 0)
                    {
                        cell_button.Enabled = true;
                        cell_button.Click += new EventHandler(OnCheckerPress);
                        if (map[i, j] == 1) { cell_button.Image = white_figure; }
                        if (map[i, j] == 2) { cell_button.Image = black_figure; }
                        cell_button.BackColor = Color.Gray;
                    }
                    else
                    {
                        cell_button.Enabled = false;
                        cell_button.BackColor = Color.White;
                    }
                    all_buttons[i, j] = cell_button;
                    this.Controls.Add(cell_button);
                }
            }
        }
        public void ChangePlayer()
        {
            if (current_player == 1)
                current_player = 2;
            else
                current_player = 1;
        }
        public void OnCheckerPress(object sender, EventArgs e)
        {
            if (prev_button != null)
            {
                if (prev_button.Location.Y / cell_size % 2 != 0 && prev_button.Location.X / cell_size % 2 == 0 || prev_button.Location.Y / cell_size % 2 == 0 && prev_button.Location.X / cell_size % 2 != 0)
                {
                    prev_button.BackColor = Color.Gray;
                }
                else
                {
                    prev_button.BackColor = Color.White;
                }
            }

            pressed_button = sender as Button;

            if (map[pressed_button.Location.Y / cell_size, pressed_button.Location.X / cell_size] == current_player)
            {
                CloseChecker();
                pressed_button.BackColor = Color.Red;
                DeactivateAllButtons();
                pressed_button.Enabled = true;
                countEatSteps = 0;
                if (pressed_button.Text == "Q")
                    ShowSteps(pressed_button.Location.Y / cell_size, pressed_button.Location.X / cell_size, false);
                else ShowSteps(pressed_button.Location.Y / cell_size, pressed_button.Location.X / cell_size,true);


                if (isMoving)
                {
                    CloseChecker();
                    if (pressed_button.Location.Y / cell_size % 2 != 0 && pressed_button.Location.X / cell_size % 2 == 0 || pressed_button.Location.Y / cell_size % 2 == 0 && pressed_button.Location.X / cell_size % 2 != 0)
                    {
                        pressed_button.BackColor = Color.Gray;
                    }
                    else
                    {
                        pressed_button.BackColor = Color.White;
                    }
                    ShowPossibleSteps();
                    isMoving = false;
                }
                else
                    isMoving = true;
            }
            else
            {
                if (isMoving)
                {
                    isContinue = false;
                    if (Math.Abs(pressed_button.Location.X / cell_size - prev_button.Location.X / cell_size) > 1)
                    {
                        isContinue = true;
                        DeleteEaten(pressed_button, prev_button);
                        if(current_player==1)
                        {
                            score_white++;
                            score1.Text = score_white.ToString();
                        }
                        else
                        {
                            score_black++;
                            score2.Text = score_black.ToString();
                        }
                    }
                    int tmp = map[pressed_button.Location.Y / cell_size, pressed_button.Location.X / cell_size];
                    map[pressed_button.Location.Y / cell_size, pressed_button.Location.X / cell_size] = map[prev_button.Location.Y / cell_size, prev_button.Location.X / cell_size];
                    map[prev_button.Location.Y / cell_size, prev_button.Location.X / cell_size] = tmp;
                    pressed_button.Image = prev_button.Image;
                    prev_button.Image = null;
                    pressed_button.Text = prev_button.Text;
                    prev_button.Text = "";
                    QuenButton(pressed_button);
                    countEatSteps = 0;
                    isMoving = false;
                    CloseChecker();
                    DeactivateAllButtons();
                    if (pressed_button.Text == "Q")
                        ShowSteps(pressed_button.Location.Y / cell_size, pressed_button.Location.X / cell_size, false);
                    else ShowSteps(pressed_button.Location.Y / cell_size, pressed_button.Location.X / cell_size,true);
                    if (countEatSteps == 0 || !isContinue)
                    {
                        CloseChecker();
                        ChangePlayer();
                        ShowPossibleSteps();
                        isContinue = false;
                    }
                    else if (isContinue)
                    {
                        pressed_button.BackColor = Color.Red;
                        pressed_button.Enabled = true;
                        isMoving = true;
                    }
                }
            }
            prev_button = pressed_button;
        }

        public void ShowPossibleSteps()
        {
            bool isOneStep = true;
            bool isEatStep = false;
            DeactivateAllButtons();
            for (int i = 0; i < map_size; i++)
            {
                for (int j = 0; j < map_size; j++)
                {
                    if (map[i, j] == current_player)
                    {
                        if (all_buttons[i, j].Text == "Q")
                            isOneStep = false;
                        else isOneStep = true;
                        if (IsButtonHasEatStep(i, j, isOneStep, new int[2] { 0, 0 }))
                        {
                            isEatStep = true;
                            all_buttons[i, j].Enabled = true;
                        }
                    }
                }
            }
            if (!isEatStep)
                ActivateAllButtons();
        }

        public void QuenButton(Button button)
        {
            if (map[button.Location.Y / cell_size, button.Location.X / cell_size] == 2 && button.Location.Y / cell_size == map_size - 1)
            {
                button.Text = "Q";

            }
            if (map[button.Location.Y / cell_size, button.Location.X / cell_size] == 1 && button.Location.Y / cell_size == 0)
            {
                button.Text = "Q";
            }
        }

        public void DeleteEaten(Button endButton, Button startButton)
        {
            int count = Math.Abs(endButton.Location.Y / cell_size - startButton.Location.Y / cell_size);
            int startIndexX = endButton.Location.Y / cell_size - startButton.Location.Y / cell_size;
            int startIndexY = endButton.Location.X / cell_size - startButton.Location.X / cell_size;
            if(startIndexX>0)
            {
                startIndexX = 1;
            }
            else
            {
                startIndexX = -1;
            }
            if(startIndexY>0)
            {
                startIndexY = 1;
            }
            else
            {
                startIndexY = -1;
            }
            int currCount = 0;
            int i = startButton.Location.Y / cell_size + startIndexX;
            int j = startButton.Location.X / cell_size + startIndexY;
            while (currCount < count - 1)
            {
                map[i, j] = 0;
                all_buttons[i, j].Image = null;
                all_buttons[i, j].Text = "";
                i += startIndexX;
                j += startIndexY;
                currCount++;
            }
        }

        public void ShowSteps(int iCurrPos, int jCurrPos, bool isOnestep)
        {
            easyStep.Clear();
            ShowDiagonal(iCurrPos, jCurrPos, isOnestep);
            if (countEatSteps > 0)
                CloseNotEatenSteps(easyStep);
        }

        public void ShowDiagonal(int iCurrPos, int jCurrPos, bool isOneStep)
        {
            int j = jCurrPos + 1;
            for (int i = iCurrPos - 1; i >-1; i--)
            {
                if (current_player == 2 && isOneStep && !isContinue) break;
                if (IsInsideBorders(i, j))
                {
                    if (!EatenOrNotEatenWay(i, j))
                        break;
                }
                if (j < 7)
                    j++;
                else break;

                if (isOneStep)
                    break;
            }

            j = jCurrPos - 1;
            for (int i = iCurrPos - 1; i >-1; i--)
            {
                if (current_player == 2 && isOneStep && !isContinue) break;
                if (IsInsideBorders(i, j))
                {
                    if (!EatenOrNotEatenWay(i, j))
                        break;
                }
                if (j > 0)
                    j--;
                else break;

                if (isOneStep)
                    break;
            }

            j = jCurrPos - 1;
            for (int i = iCurrPos + 1; i < 8; i++)
            {
                if (current_player == 1 && isOneStep && !isContinue) break;
                if (IsInsideBorders(i, j))
                {
                    if (!EatenOrNotEatenWay(i, j))
                        break;
                }
                if (j > 0)
                    j--;
                else break;

                if (isOneStep)
                    break;
            }

            j = jCurrPos + 1;
            for (int i = iCurrPos + 1; i < 8; i++)
            {
                if (current_player == 1 && isOneStep && !isContinue) break;
                if (IsInsideBorders(i, j))
                {
                    if (!EatenOrNotEatenWay(i, j))
                        break;
                }
                if (j < 7)
                    j++;
                else break;

                if (isOneStep)
                    break;
            }
        }

        public bool EatenOrNotEatenWay(int i, int j)
        {

            if (map[i, j] == 0 && !isContinue)//проверка на существование еще одного съедобного хода шашки
            {
                all_buttons[i, j].BackColor = Color.Yellow;
                all_buttons[i, j].Enabled = true;
                easyStep.Add(all_buttons[i, j]);
            }
            else
            {

                if (map[i, j] != current_player)
                {
                    if (pressed_button.Text == "D")
                        NextEatenStep(i, j, false);
                    else NextEatenStep(i, j,true);
                }

                return false;
            }
            return true;
        }

        public void CloseNotEatenSteps(List<Button> close_buttons)
        {
            if(close_buttons.Count>0)
            {
                for(int i=0;i<close_buttons.Count;i++)
                {
                    if (close_buttons[i].Location.Y / cell_size % 2 != 0 && close_buttons[i].Location.X / cell_size % 2 == 0 || close_buttons[i].Location.Y / cell_size % 2 == 0 && close_buttons[i].Location.X / cell_size % 2 != 0)
                    {
                        close_buttons[i].BackColor = Color.Gray;
                    }
                    else
                    {
                        close_buttons[i].BackColor = Color.White;
                    }
                    close_buttons[i].Enabled = false;
                }
            }
        }

        public void NextEatenStep(int i, int j, bool isOneStep)
        {
            int way_x = i - pressed_button.Location.Y / cell_size;
            int way_y=j-pressed_button.Location.X / cell_size;
            if(way_x>0)
            {
                way_x = 1;
            }
            else
            {
                way_x = -1;
            }
            if (way_y > 0)
            {
                way_y = 1;
            }
            else
            {
                way_y = -1;
            }
            int i_next = i;
            int j_next = j;
            bool isEmpty=true;
            while(IsInsideBorders(i_next,j_next))
            {
                if (map[i_next,j_next]!=0 && map[i_next,j_next]!=current_player)
                {
                    isEmpty = false;
                    break;
                }
                i_next += way_x;
                j_next += way_y;
                if (isOneStep)
                    break;
            }
            if (isEmpty)
                return;
            
                List<Button> close_buttons = new List<Button>();
                bool closeIsNotEaten = false;
                int i_move = i_next + way_x;
                int j_move = j_next + way_y;
                while(IsInsideBorders(i_move, j_move))
                {
                if (map[i_move, j_move] == 0)
                {
                    if (IsButtonHasEatStep(i_move, j_move, isOneStep, new int[2] { way_x, way_y }))
                    {
                        closeIsNotEaten = true;
                    }
                    else
                    {
                        close_buttons.Add(all_buttons[i_move, j_move]);
                    }
                    all_buttons[i_move, j_move].BackColor = Color.Yellow;
                    all_buttons[i_move, j_move].Enabled = true;
                    countEatSteps++;
                }
                else break;
                if (isOneStep) break;
                    i_move += way_x;
                    j_move += way_y;
                }
                if(closeIsNotEaten && close_buttons.Count>0)
                {
                    CloseNotEatenSteps(close_buttons);
                }
            
        }

        public bool IsButtonHasEatStep(int iCurrPos, int jCurrPos, bool isNotQueen, int[] way)
        {
            bool eatStep = false;
            int j = jCurrPos + 1;
            for (int i = iCurrPos - 1; i > -1; i--)//движение шашки вверх вправо
            {
                if (current_player == 2 && isNotQueen && !isContinue) break;
                if (way[0] == 1 && way[1] == -1 && !isNotQueen) break;
                if (IsInsideBorders(i, j))
                {
                    if (map[i, j] != 0 && map[i, j] != current_player)
                    {
                        eatStep = true;
                        
                        if (!IsInsideBorders(i - 1, j + 1) || (map[i - 1, j + 1] != 0))
                        {
                            eatStep = false;
                        }
                        else
                        {
                            return eatStep;
                        }
                    }
                }
                if (j < 7) j++;
                else break;
                if (isNotQueen) break;
            }
            j = jCurrPos - 1;
            for (int i = iCurrPos - 1; i > -1; i--)//движение шашки вверх влево
            {
                if (current_player == 2 && isNotQueen && !isContinue) break;
                if (way[0] == 1 && way[1] == 1 && !isNotQueen) break;
                if (IsInsideBorders(i, j))
                {
                    if (map[i, j] != 0 && map[i, j] != current_player)
                    {
                        eatStep = true;
                        if (!IsInsideBorders(i - 1, j - 1) || (map[i - 1, j - 1] != 0))
                        {
                            eatStep = false;
                        }
                        else
                        {
                            return eatStep;
                        }
                    }
                }
                if (j > 0) j--;
                else break;
                if (isNotQueen) break;
            }
            j = jCurrPos - 1;
            for (int i = iCurrPos + 1; i < 8; i++)//движение шашки вниз влево
            {
                if (current_player == 1 && isNotQueen && !isContinue) break;
                if (way[0] == -1 && way[1] == 1 && !isNotQueen) break;
                if (IsInsideBorders(i, j))
                {
                    if (map[i, j] != 0 && map[i, j] != current_player)
                    {
                        eatStep = true;
                        if (!IsInsideBorders(i + 1, j - 1) || (map[i + 1, j - 1] != 0))
                        {
                            eatStep = false;
                        }
                        else
                        {
                           
                            return eatStep;
                        }
                    }
                }
                if (j > 0) j--;
                else break;
                if (isNotQueen) break;
            }
            j = jCurrPos + 1;
            for (int i = iCurrPos + 1; i < 8; i++)//движение шашки вниз вправо
            {
                if (current_player == 1 && isNotQueen && !isContinue) break;
                if (way[0] == -1 && way[1] == -1 && !isNotQueen) break;
                if (IsInsideBorders(i, j))
                {
                    if (map[i, j] != 0 && map[i, j] != current_player)
                    {
                        eatStep = true;
                        if (!IsInsideBorders(i + 1, j + 1) || (map[i + 1, j + 1] != 0))
                        {
                            eatStep = false;
                        }
                        else
                        {
                            return eatStep;
                        }
                    }
                }
                if (j < 7) j++;
                else break;
                if (isNotQueen) break;
            }
            return eatStep;
        }

        public void CloseChecker()
        {
            for (int i = 0; i < map_size; i++)
            {
                for (int j = 0; j < map_size; j++)
                {
                    if (all_buttons[i, j].Location.Y / cell_size % 2 != 0 && all_buttons[i, j].Location.X / cell_size % 2 == 0 || all_buttons[i, j].Location.Y / cell_size % 2 == 0 && all_buttons[i, j].Location.X / cell_size % 2 != 0)
                    {
                        all_buttons[i, j].BackColor = Color.Gray;
                    }
                    else
                    {
                        all_buttons[i, j].BackColor = Color.White;
                    }
                }
            }
        }

        public bool IsInsideBorders(int i, int j)
        {
            if (i >= map_size || i < 0 || j >= map_size || j < 0)
            {
                return false;
            }  
            return true;
        }

        public void ActivateAllButtons()
        {
            for (int i = 0; i < map_size; i++)
            {
                for (int j = 0; j < map_size; j++)
                {
                    all_buttons[i, j].Enabled = true;
                }
            }
        }

        public void DeactivateAllButtons()
        {
            for (int i = 0; i < map_size; i++)
            {
                for (int j = 0; j < map_size; j++)
                {
                    all_buttons[i, j].Enabled = false;
                }
            }
        }
    }
}
