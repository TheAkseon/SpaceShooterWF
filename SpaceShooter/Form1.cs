using System;
using System.Drawing;
using System.Windows.Forms;
using WMPLib;

namespace SpaceShooter
{
    public partial class Form1 : Form
    {
        private WindowsMediaPlayer backgroundSound;
        private WindowsMediaPlayer shootSound;
        private WindowsMediaPlayer explosionSound;

        private int enemySpeed;
        private int backgroundSpeed;
        private int playerSpeed;
        private int munitionSpeed;
        private int enemiesMunitionSpeed;

        private int score;
        private int level;
        private int difficulty;

        private bool isPause;
        private bool isGameOver;

        private Random random;

        private PictureBox[] enemies;
        private PictureBox[] munitions;
        private PictureBox[] stars;
        private PictureBox[] enemiesMunitions;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            isPause = false;
            isGameOver = false;
            score = 0;
            level = 1;
            difficulty = 9;

            backgroundSpeed = 4;
            playerSpeed = 5;
            enemySpeed = 5;
            munitionSpeed = 20;
            enemiesMunitionSpeed = 4;

            Image munition = Image.FromFile(@"assets\munition.png");

            Image enemy1 = Image.FromFile("assets\\Enemy1.png");
            Image enemy2 = Image.FromFile("assets\\Enemy2.png");
            Image enemy3 = Image.FromFile("assets\\Enemy3.png");
            Image enemy4 = Image.FromFile("assets\\Enemy4.png");
            Image enemy5 = Image.FromFile("assets\\Enemy5.png");

            enemies = new PictureBox[10];
            stars = new PictureBox[20];
            munitions = new PictureBox[3];
            enemiesMunitions = new PictureBox[10];

            random = new Random();

            Player.Location = new Point(200, 340);

            backgroundSound = new WindowsMediaPlayer();
            shootSound = new WindowsMediaPlayer();
            explosionSound = new WindowsMediaPlayer();


            backgroundSound.URL = "songs\\GameSong.mp3";
            shootSound.URL = "songs\\shoot.mp3";
            explosionSound.URL = "songs\\boom.mp3";

            backgroundSound.settings.setMode("loop", true);
            backgroundSound.settings.volume = 5;
            shootSound.settings.volume = 1;
            explosionSound.settings.volume = 6;

            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i] = new PictureBox();
                enemies[i].Size = new Size(25, 25);
                enemies[i].SizeMode = PictureBoxSizeMode.Zoom;
                enemies[i].Visible = false;
                Controls.Add(enemies[i]);
                enemies[i].Location = new Point((i + 1) * 35, -50);
            }

            enemies[0].Image = enemy4;
            enemies[1].Image = enemy1;
            enemies[2].Image = enemy2;
            enemies[3].Image = enemy3;
            enemies[4].Image = enemy1;
            enemies[5].Image = enemy3;
            enemies[6].Image = enemy2;
            enemies[7].Image = enemy3;
            enemies[8].Image = enemy1;
            enemies[9].Image = enemy5;

            for (int i = 0; i < munitions.Length; i++)
            {
                munitions[i] = new PictureBox();
                munitions[i].Size = new Size(6, 6);
                munitions[i].Image = munition;
                munitions[i].SizeMode = PictureBoxSizeMode.Zoom;
                munitions[i].BorderStyle = BorderStyle.None;
                Controls.Add(munitions[i]);
            }

            for (int i = 0; i < stars.Length; i++)
            {
                stars[i] = new PictureBox();
                stars[i].BorderStyle = BorderStyle.None;
                stars[i].Location = new Point(random.Next(20, 400), random.Next(10, 400));

                if(i % 2 == 1)
                {
                    stars[i].Size = new Size(2, 2);
                    stars[i].BackColor = Color.Wheat;
                }
                else
                {
                    stars[i].Size = new Size(3, 3);
                    stars[i].BackColor = Color.DarkGray;
                }

                Controls.Add(stars[i]);
            }

            for (int i = 0; i < enemiesMunitions.Length; i++)
            {
                int randomEnemy;

                enemiesMunitions[i] = new PictureBox();
                enemiesMunitions[i].Size = new Size(2, 25);
                enemiesMunitions[i].Visible = false;
                enemiesMunitions[i].BackColor = Color.Yellow;

                randomEnemy = random.Next(0, enemies.Length);
                enemiesMunitions[i].Location = new Point(enemies[randomEnemy].Location.X, enemies[randomEnemy].Location.Y - 20);
                Controls.Add(enemiesMunitions[i]);
            }

            backgroundSound.controls.play();
        }

        private void MoveBackgroundTimer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].Top += backgroundSpeed;

                if(stars[i].Top >= Height)
                {
                    stars[i].Top = -stars[i].Height;
                }
            }

            for (int i = stars.Length; i < stars.Length; i++)
            {
                stars[i].Top += backgroundSpeed - 2;

                if(stars[i].Top >= Height)
                {
                    stars[i].Top = -stars[i].Height;
                }
            }
        }

        private void LeftMoveTimer_Tick(object sender, EventArgs e)
        {
            if(Player.Left > 10)
            {
                Player.Left -= playerSpeed;
            }
        }

        private void RightMoveTimer_Tick(object sender, EventArgs e)
        {
            if(Player.Right < 375)
            {
                Player.Left += playerSpeed;
            }
        }

        private void DownMoveTimer_Tick(object sender, EventArgs e)
        {
            if(Player.Top < 350)
            {
                Player.Top += playerSpeed;
            }
        }

        private void UpMoveTimer_Tick(object sender, EventArgs e)
        {
            if(Player.Top > 10)
            {
                Player.Top -= playerSpeed;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (isPause == false)
            {
                if (e.KeyCode == Keys.Right)
                {
                    RightMoveTimer.Start();
                }

                if (e.KeyCode == Keys.Left)
                {
                    LeftMoveTimer.Start();
                }

                if (e.KeyCode == Keys.Down)
                {
                    DownMoveTimer.Start();
                }

                if (e.KeyCode == Keys.Up)
                {
                    UpMoveTimer.Start();
                }
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            RightMoveTimer.Stop();
            LeftMoveTimer.Stop();
            DownMoveTimer.Stop();
            UpMoveTimer.Stop();

            if(e.KeyCode == Keys.Space)
            {
                if(isGameOver == false)
                {
                    if(isPause == true)
                    {
                        StartTimers();
                        Label.Visible = false;
                        backgroundSound.controls.play();
                        isPause = false;
                    }
                    else
                    {
                        Label.Text = "Paused";
                        Label.Visible = true;
                        backgroundSound.controls.pause();
                        StopTimers();
                        isPause = true;
                    }
                }
            }
        }

        private void MoveMunitionTimer_Tick(object sender, EventArgs e)
        {
            shootSound.controls.play();

            for (int i = 0; i < munitions.Length; i++)
            {
                if(munitions[i].Top > 0)
                {
                    munitions[i].Visible = true;
                    munitions[i].Top -= munitionSpeed;

                    CollisionEnemy();
                }
                else
                {
                    munitions[i].Visible = false;
                    munitions[i].Location = new Point(Player.Location.X + 10, Player.Location.Y - i * 30);
                }
            }
        }

        private void MoveEnemiesTimer_Tick(object sender, EventArgs e)
        {
            MoveEnemies();
        }

        private void MoveEnemies()
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].Visible = true;
                enemies[i].Top += enemySpeed;

                if(enemies[i].Top > Height)
                {
                    enemies[i].Location = new Point((i + 1) * 35, -50);
                }
            }
        }

        private void CollisionEnemy()
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                for (int j = 0; j < munitions.Length; j++)
                {
                    if (munitions[j].Bounds.IntersectsWith(enemies[i].Bounds))
                    {
                        explosionSound.controls.play();

                        score += 1;
                        ScoreCount.Text = score.ToString();

                        if(score % 30 == 0)
                        {
                            level += 1;
                            LevelCount.Text = level.ToString(); 

                            if(level < 10)
                            {
                                difficulty--;
                                enemySpeed++;
                                enemiesMunitionSpeed++;
                            }
                            else
                            {
                                GameOver("Nice done");
                            }
                        }
                        enemies[i].Location = new Point((i + 1) * 35, -50);
                    }
                }

                if (Player.Bounds.IntersectsWith(enemies[i].Bounds))
                {
                    explosionSound.settings.volume = 30;
                    explosionSound.controls.play();
                    Player.Visible = false;
                    GameOver("Game Over!");
                }
            }
        }

        private void CollisionWithEnemiesMuntition()
        {
            for (int i = 0; i < enemiesMunitions.Length; i++)
            {
                if (enemiesMunitions[i].Bounds.IntersectsWith(Player.Bounds))
                {
                    enemiesMunitions[i].Visible = false;
                    explosionSound.settings.volume = 30;
                    explosionSound.controls.play();
                    Player.Visible = false;
                    GameOver("Game Over!");
                }
            }
        }

        private void GameOver(string word)
        {
            Label.Text = word;
            Label.Visible = true;
            RestartButton.Visible = true;
            ExitButton.Visible = true;

            backgroundSound.controls.stop();
            StopTimers();
        }

        private void StopTimers()
        {
            MoveBackgroundTimer.Stop();
            MoveEnemiesTimer.Stop();
            MoveMunitionTimer.Stop();
            EnemiesMunitionTimer.Stop();
        }

        private void StartTimers()
        {
            MoveBackgroundTimer.Start();
            MoveEnemiesTimer.Start();
            MoveMunitionTimer.Start();
            EnemiesMunitionTimer.Start();
        }

        private void EnemiesMunitionTimer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < (enemiesMunitions.Length - difficulty); i++)
            {
                if (enemiesMunitions[i].Top < Height)
                {
                    enemiesMunitions[i].Visible = true;
                    enemiesMunitions[i].Top += enemiesMunitionSpeed;

                    CollisionWithEnemiesMuntition();
                }
                else
                {
                    enemiesMunitions[i].Visible = false;
                    int randomEnemy = random.Next(0, enemies.Length);
                    enemiesMunitions[i].Location = new Point(enemies[randomEnemy].Location.X + 20, enemies[randomEnemy].Location.Y + 30);
                }
            }
        }

        private void RestartButton_Click(object sender, EventArgs e)
        {
            Controls.Clear();
            InitializeComponent();
            Form1_Load(e, e);
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Environment.Exit(1);
        }
    }
}
