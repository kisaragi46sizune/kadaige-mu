using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kadaige_mu
{
    public partial class Form1 : Form
    {
        static Random rand = new Random();
        //敵の速度
        const float ENEMY_SPEED = 10f;
        //アイテムの最大速度
        const float ITEM_SPEED = 10f;
        //アイテムの残り数
        int iItemCount = 0;
        enum SCENES
        {
            SC_NONE,//無効
            SC_BOOT,//起動
            SC_TITLE,//タイトル
            SC_GAME,//ゲーム中
            SC_GAMEOVER,//ゲームオーバー
            SC_CLEAR,//クリア
        };
        /**現在のシーン*/
        SCENES nowScene = SCENES.SC_NONE;
        /**次のシーン*/
        SCENES nextScene = SCENES.SC_BOOT;

        /**敵の上限数*/
        const int ENEMY_MAX = 10;
        /**アイテムの上限値*/
        const int ITEM_MAX = 10;
        /**キャラクターの上限数*/
        const int CHR_MAX = 1 + ENEMY_MAX + ITEM_MAX;

        /**キャラクタータイプ*/
        enum CHRTIPYPE
        {
            CHRTIPYPE_NONE,
            CHRTIPYPE_PLAYER,
            CHRTIPYPE_ENEMY,
            CHRTIPYPE_ITEM
        }

        /**キャラクタータイプ*/
        CHRTIPYPE[] type = new CHRTIPYPE[CHR_MAX];
        /**X座標*/
        float[] px = new float[CHR_MAX];
        /**Yざひょう*/
        float[] py = new float[CHR_MAX];
        /**X速度*/
        float[] vx = new float[CHR_MAX];
        /**Y速度*/
        float[] vy = new float[CHR_MAX];
        /**ラベル*/
        Label[] labels = new Label[CHR_MAX];

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        /**初期化*/
        private void init()
        {
            // シーンの切り替えが必要か
            if (nextScene == SCENES.SC_NONE)
                return;//init()をでる
            // シーンの切り替えが必要
            nowScene = nextScene;
            nextScene = SCENES.SC_NONE;

            switch (nowScene)
            {
                // 起動
                case SCENES.SC_BOOT:
                    for (int i = 0; i < CHR_MAX; i++)
                    {
                        type[i] = CHRTIPYPE.CHRTIPYPE_NONE;
                        labels[i] = new Label();
                        labels[i].Visible = false;
                        labels[i].AutoSize = true;
                        Controls.Add(labels[i]);
                     }
                    nextScene = SCENES.SC_TITLE;
                    break;

                //ゲーム開始時の初期化
                case SCENES.SC_GAME:
                    type[0] = CHRTIPYPE.CHRTIPYPE_PLAYER;
                    vx[0] = 0;
                    vy[0] = 0;
                    labels[0].Text = "(・ω・)";
                    labels[0].Visible = true;
                    px[0] = (ClientSize.Width - labels[0].Width) / 2;
                    py[0] = (ClientSize.Height - labels[0].Height) / 2;
                    labels[0].Left = (int)px[0];
                    labels[0].Top = (int)py[0];

                    //敵の初期化
                    for (int i = 1; i < 1 + ENEMY_MAX; i++)
                    {
                        type[i] = CHRTIPYPE.CHRTIPYPE_ENEMY;
                        vx[i] = (float)(rand.NextDouble() * (2 * ENEMY_SPEED) - ENEMY_SPEED);
                        vy[i] = (float)(rand.NextDouble() * (2 * ENEMY_SPEED) - ENEMY_SPEED);
                        labels[i].Text = "♂";
                        px[i] = rand.Next(ClientSize.Width - labels[i].Width);
                        py[i] = rand.Next(ClientSize.Height - labels[i].Height);
                    }

                    //アイテムの初期化
                    for (int i = 1+ENEMY_MAX; i < CHR_MAX; i++)
                    {
                        type[i] = CHRTIPYPE.CHRTIPYPE_ITEM;
                        vx[i] = (float)(rand.NextDouble() * (2 * ITEM_SPEED) - ITEM_SPEED);
                        vy[i] = (float)(rand.NextDouble() * (2 * ITEM_SPEED) - ITEM_SPEED);
                        labels[i].Text = "♀";
                        px[i] = rand.Next(ClientSize.Width - labels[i].Width);
                        py[i] = rand.Next(ClientSize.Height - labels[i].Height);
                    }
                    //アイテムの残り数を設定
                    iItemCount = ITEM_MAX;



                    break;
            }
        }

        /**更新処理*/
        private void updata()
        {
            switch (nowScene)
            {
                case SCENES.SC_TITLE:
                    if ((Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left)
                    {
                        //左クリック
                        nextScene = SCENES.SC_GAME;
                    }
                    break;
                case SCENES.SC_GAME:
                    updateGame();
                    break;
                case SCENES.SC_GAMEOVER:
                case SCENES.SC_CLEAR:
                    if ((Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left)
                    {
                        //左クリック
                        nextScene = SCENES.SC_TITLE;
                    }
                    break;
            }
        }
        /**ゲームの更新処理*/
        private void updateGame()
        {
            for (int i = 0; i < CHR_MAX; i++)
            {
                switch (type[i])
                {
                    case CHRTIPYPE.CHRTIPYPE_PLAYER:
                        updataPlayer(i);
                        break;
                    case CHRTIPYPE.CHRTIPYPE_ENEMY:
                        updateEnemy(i);
                        break;
                    case CHRTIPYPE.CHRTIPYPE_ITEM:
                        updateItem(i);
                        break;
                }
            }
        }
        //敵の更新処理
        private void updateEnemy(int i)
        {
            constantMove(i);
            if (hitPlayer(i))
            {
                nextScene = SCENES.SC_GAMEOVER;
            }
        }

        //等速直線運動
        private void constantMove(int i)
        {
            px[i] += vx[i];
            py[i] += vy[i];


            if (px[i] <= 0)
            {
                vx[i] = Math.Abs(vx[i]);
            }

            else if (px[i] > ClientSize.Width - labels[i].Width)
            {
                vx[i] = -Math.Abs(vx[i]);
            }

            if (py[i] <= 0)
            {
                vy[i] = Math.Abs(vy[i]);
            }

            else if (py[i] > ClientSize.Height - labels[i].Height)
            {
                vy[i] = -Math.Abs(vy[i]);
            }


        }
        //アイテムの更新処理
        private void updateItem(int i)
        {
            constantMove(i);
            if (hitPlayer(i))
            {
                //アイテムを消す
                type[i] = CHRTIPYPE.CHRTIPYPE_NONE;
                iItemCount--;
                if (iItemCount <= 0)
                {
                    //アイテムをすべてとった
                    nextScene = SCENES.SC_CLEAR;
                }
            }
        }

        //pyレイヤーがぶつかっているか
        //true=ぶつかってる false=ぶつかって内
        private bool hitPlayer(int i)
        {
            
            if ((px[0]<labels[i].Right)&&(labels[0].Right>px[i])&&(py[0]<labels[i].Bottom)&&(labels[0].Bottom>py[i]))
            {
                return true;
            }
            return false;
        }
        /**プレイヤーの更新処理*/
        private void updataPlayer(int i)
        {
            Point cpos;
            cpos = PointToClient(MousePosition);
            px[0] = cpos.X-labels[0].Width/2;
            py[0] = cpos.Y -labels[0].Height/2;
        }

        /**描画*/
        private void render()
        {
            switch (nowScene)
            {
                case SCENES.SC_TITLE:
                    Text = "TITLE";
                    break;
                case SCENES.SC_GAME:
                    Text = "GAME";
                    for (int i = 0; i < CHR_MAX; i++)
                    {
                        if (type[i] != CHRTIPYPE.CHRTIPYPE_NONE)
                        {
                            labels[i].Visible = true;
                            labels[i].Left = (int)px[i];
                            labels[i].Top = (int)py[i];
                        }
                        else
                        {
                            labels[i].Visible = false;
                        }
                    }
                    break;
                case SCENES.SC_GAMEOVER:
                    Text = "GAMEOVER";
                    break;
                case SCENES.SC_CLEAR:
                    Text = "CLEAR";
                    break;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            init();
            updata();
            render();
        }
    }
}
