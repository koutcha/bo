using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApplication7.Properties;

namespace WindowsFormsApplication7
{
    
    
    public partial class Form1 : Form
    {

        private int fps;
        private int oldtime;

        Label label1 = new Label();

        MainGame maingame = new MainGame(new KeyManager());

        public static int formWedth = 1000;
        public static int formHeight = 1000;




        public Form1() : base()
        {
            fps = 0;
            oldtime = System.Environment.TickCount;
            SetStyle(
                ControlStyles.DoubleBuffer |
                ControlStyles.UserPaint |
                 ControlStyles.AllPaintingInWmPaint, true
           );
           

            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            this.Size = new Size(formWedth, formHeight);
            this.BackColor = Color.Black;

            

            this.KeyDown += MyForm_keyDown;
            this.KeyUp += MyForm_keyup;
            Timer timer = new Timer();
            timer.Interval = 15;
            timer.Tick += new EventHandler(loops);
            timer.Start();


        }

        public void RenderFps()
        {
            fps++;
            if (System.Environment.TickCount >= oldtime + 1000)
            {
                oldtime = System.Environment.TickCount;
                this.label1.Text = fps.ToString();
                fps = 0;
            }
        }


        private void loops(object sender, EventArgs e)
        {
            maingame.loop();
            Invalidate();
        }

        private void MyForm_keyup(object sender, KeyEventArgs e)
        {
            
          maingame.keyUpCatcher(e);
        }

        private void MyForm_keyDown(object sender, KeyEventArgs e)
        {
           maingame.keyDownCatcher(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
           maingame.mainPainter(e);
        }
    }

  
    
   
    class ControlAssignedKey
    {

        List<Keys> assigningKeys = new List<Keys>();
        KeyRegistry keyRegistry;
        
        



        ControlAssignedKey(Keys[] keys)
        {

            for (int i = 0; i < keys.GetLength(0); i++)
            {
                assigningKeys.Add(keys[i]);
            }
        }

        public bool IsAsigningKeysOn()
        {
            bool checkStateSum = true;
            foreach (Keys key in assigningKeys)
            {

                if (KeyRegistry.KeysState.Onkey == keyRegistry.getCurrentKeyStatus(key))
                {
                    checkStateSum = checkStateSum && true;
                }
                else
                {
                    checkStateSum = checkStateSum && false;
                }
            }
            return checkStateSum;
        }


    }


    //タイトル画面
    class Title
    {
        Image titleBackImg;

        SolidBrush titleRogoColor = new SolidBrush(Color.Yellow);
        Font titleRogofont = new Font("MS UI Gothic", 90);
        String titleRogoName = "BreakBlock";


        

        int animationCount;
        int subRogoheight = 1000;
        public Title(Image titleBackImg)
        {
            this.titleBackImg = titleBackImg;
        }
        public void sequance()
        {

        }

        public void painter(PaintEventArgs e)
        {
            animationCount++;
            e.Graphics.DrawImage(titleBackImg, 0, 0, 1000, 1100);
            e.Graphics.DrawString(titleRogoName, titleRogofont, titleRogoColor, 170, 120);
           
           
             

            if (subRogoheight > 500)
            {

                e.Graphics.DrawString("PushEnter!", titleRogofont, titleRogoColor, 240, subRogoheight);
                subRogoheight -=3;
            }else if (animationCount>20)
            {
                e.Graphics.DrawString("PushEnter!", titleRogofont, titleRogoColor, 240, subRogoheight);
            }

                if (animationCount > 60)
                {
                    animationCount = 0;
                }

            }
           
    }


    //メイン部分（実際に遊ぶ所）
    class MainGame
    {

        Title title;
        Ball ball;
        Bar bar;
        private Block[,] blocks;
        BackgroundRect bg;

        int totalScore;
        //当たり判定
        Collision collision;

        private KeyManager keymanager;
        private KeyManage keymanage = new KeyManage(new Keys[] { Keys.Enter });
        
        private int framecount = 0;

        //状態（準備状態、プレイ中、ゲームオーバー、ゲームクリア
        StatusOnGame onGameState;

        //残機

        int plife;

        //球の速さ
        int Ballspeed = -10;

        //タイトル画面に戻れるかどうかの判別用
        bool changeTitleFlag = false;

        
        public bool isPossibleToChengeTitle
        {
            get { return this.changeTitleFlag; }
        }

        
        public MainGame(KeyManager keymanager)
        {
            this.keymanager = keymanager;
            this.title = new Title(Resources.IMG_0013);
            onGameState = StatusOnGame.Title;
            reset();
        }
        
        public void loop()
        {
            framecount++;
            keymanage.UpdateKeyFrames();
            checkKeyEffectGaming();

            switch (this.onGameState){
                case StatusOnGame.Title:


                    break;
                case StatusOnGame.Preparation:

                    ball.StickingBarMotion(bar);
                    bar.Motion();
                    bar.overBackgroundCheck(bg);

                    break;
                case StatusOnGame.Playing:

                    bar.Motion();
                    ball.Motion();
                    bar.overBackgroundCheck(bg);
                    collision.ballAndWall();
                    collision.ballAndBar();
                    collision.ballAndBlock();
                    totalScore = totalScore + collision.getScore();
                    GameOverCheck();
                    GameclearCheck();
                    break;
                case StatusOnGame.GameClear:
                    break;
                case StatusOnGame.GameOver:
                    break;
            }
        }

        public void keyDownCatcher(KeyEventArgs e)
        {
            this.keymanager.onKeyCheck(e);
            this.keymanage.KeyDownCatcher(e);

        }
        public void keyUpCatcher(KeyEventArgs e)
        {
            this.keymanager.releaseKeyCheck(e);
            this.keymanage.KeyUpCatcher(e);
            
        }

        public void painter(PaintEventArgs e)
        {
            switch (this.onGameState)
            {
                case StatusOnGame.Title:
                   

                    break;
                case StatusOnGame.Preparation:

                  

                    break;
                case StatusOnGame.Playing:

                  
                    break;
                case StatusOnGame.GameClear:
                    break;
                case StatusOnGame.GameOver:
                    break;
            }

        }
        public enum StatusOnGame
        {
           Title = 200,
           Preparation = 201,
           Playing =202,
           GameOver = 203,
           GameClear = 204,
        }
        public void checkKeyEffectGaming()
        {
            switch (this.onGameState)
            {
                case StatusOnGame.Title:
                    checkKeysForTitle();
                    break;
                case StatusOnGame.Preparation:
                    checkeyForPreparation();
                    break;
                case StatusOnGame.Playing:
                    checkKeysForBar();
                break;
                case StatusOnGame.GameClear:
                    checkKeyForGameClear();
                    break;
                case StatusOnGame.GameOver:
                    checkKeyForGameOver();
                    break;
            }
        }
        private void checkKeysForTitle()
        {
            if (keymanage.getKeysDownFrame(Keys.Enter)>0)
            {
                onGameState = StatusOnGame.Preparation;
            }
            
        }
        private void checkKeysForBar()
        {
            if (keymanager.OnLeft && keymanager.OnRight)
            {
                bar.stop();
            }
            else if (keymanager.OnLeft)
            {
                bar.moveL();
            }else if(keymanager.OnRight)
            {
                bar.moveR();
            }
            else
            {
                bar.stop();
            }
           
        }
        private void checkKeyForGameOver()
        {
            if (keymanager.OnEnter)
            {
                onGameState = StatusOnGame.Preparation;
                reset();
            }
            if (keymanager.OnSpace)
            {
                onGameState = StatusOnGame.Title;
                reset();
            }
        }
        private void checkeyForPreparation()
        {
            if (keymanager.OnLeft && keymanager.OnRight)
            {
                bar.stop();
            }
            else if (keymanager.OnLeft)
            {
                bar.moveL();
            }
            else if (keymanager.OnRight)
            {
                bar.moveR();
            }
            else
            {
                bar.stop();
            }
            if (keymanager.OnUp )
            {
                ball.StickingBarMotion(bar);
                onGameState = StatusOnGame.Playing;
            }
         
        }
       private void checkKeyForGameClear()
        {
            if (keymanager.OnEnter)
            {
                onGameState = StatusOnGame.Preparation;
                reset();
            }
            if(keymanager.OnSpace)
            {
                onGameState = StatusOnGame.Title;
                reset();
            }
        }

        private void GameOverCheck()
        {
            if ((ball.Y + ball.Height) > bg.Y + bg.Height)
            {
                plife = plife - 1;
                ball.Dx = Ballspeed;
                ball.Dy = Ballspeed;
                onGameState = StatusOnGame.Preparation;
                if(plife < 0)
                {
                    plife = 0;
                    onGameState = StatusOnGame.GameOver;
                  
                    bar.stop();
                }
            }
        }
        private void GameclearCheck()
        {
            int lifesum = 0;
            for (int i = 0; i < blocks.GetLength(0); i++)
            {
                for (int j = 0; j < blocks.GetLength(1); j++)
                {

                    lifesum += blocks[i, j].Life;

                }
            }

            if(lifesum == 0)
            {
                onGameState = StatusOnGame.GameClear;
                
            }
        }
        //ブロックを並べる関数、初期点は最初のブロック依存
        private void LineUpBlocks(int BlockInterval, int blockRowNumber, int blockColumnNumber, Block copySource)
        {
            int x = copySource.X;
            int y = copySource.Y;

            this.blocks = new Block[blockColumnNumber, blockRowNumber];
            {
                for (int i = 0; i < blockColumnNumber; i++)
                {
                    for (int j = 0; j < blockRowNumber; j++)
                    {
                        if (i == 0 && j == 0)
                        {
                            blocks[i, j] = copySource;
                        }
                        else
                        {
                            blocks[i, j] = new Block(x, y, copySource.Width, copySource.Height, copySource.Life);

                        }
                        x += (copySource.Width + BlockInterval);
                    }
                    x = copySource.X;
                    y += (copySource.Height + BlockInterval);
                }


            }
        }

        private void LineUpBlocks(int BlockInterval, Block copySource, int[,] blockLifeMatrix)
        {
            int x = copySource.X;
            int y = copySource.Y;

            int blockRowNumber = blockLifeMatrix.GetLength(1);
            int blockColumnNumber = blockLifeMatrix.GetLength(0);

            this.blocks = new Block[blockColumnNumber, blockRowNumber];
            {
                for (int i = 0; i < blockColumnNumber; i++)
                {
                    for (int j = 0; j < blockRowNumber; j++)
                    {
                        if (i == 0 && j == 0)
                        {
                            blocks[i, j] = copySource;
                        }
                        else
                        {
                            blocks[i, j] = new Block(x, y, copySource.Width, copySource.Height, copySource.Life);

                        }
                        x += (copySource.Width + BlockInterval);
                    }
                    x = copySource.X;
                    y += (copySource.Height + BlockInterval);
                }
            }
        
        }
       
        //初期化
        int blockPerPoint = 10;

        private void reset()
        {
            changeTitleFlag = false;
            totalScore = 0;
            plife = 2;
            LineUpBlocks(3, 11, 5, new Block(70,60,60,30,1));
            bar = new Bar(400, 900, 200, 30, 10);
            ball = new Ball(600, 100, 20, 20,Ballspeed,Ballspeed ,bar);
            bg = new BackgroundRect(10, 10, 800,1000,Resources.Image1);
            
           
            collision = new Collision(ball,blocks,bar,bg,blockPerPoint);
        }
        private void restartSetting()
        {
            plife = 2;
        }
        public void mainPainter(PaintEventArgs e)
        {
            if(this.onGameState == StatusOnGame.Title)
            {
                title.painter(e);
            }
            else
            {
                onGamePainter(e);
            }
        }
        public void onGamePainter(PaintEventArgs e)
        {



            commonPrinterOnGaming(e);

            switch (this.onGameState)
            {
            

                case StatusOnGame.Preparation:
                    SolidBrush br0 = new SolidBrush(Color.Blue);
                    Font font0 = new Font("MS UI Gothic", 70);
                    e.Graphics.DrawString("GetReady..?", font0, br0, 160, 110);
                    font0 = new Font("MS UI Gothic", 70);
                    e.Graphics.DrawString("start up Key", font0, br0, 160, 400);
                    break;
                case StatusOnGame.Playing:
                    break;
                case StatusOnGame.GameClear:
                    SolidBrush br = new SolidBrush(Color.Red);
                    Font font = new Font("MS UI Gothic", 70);
                    e.Graphics.DrawString("GameClear", font, br, 160, 110);
                    font = new Font("MS UI Gothic", 40);
                    e.Graphics.DrawString("Enter:Retry Space:End(to title)", font, br, 160, 400);
                    break;
                case StatusOnGame.GameOver:
                    SolidBrush br2 = new SolidBrush(Color.Red);
                    Font font2 = new Font("MS UI Gothic", 70);
                    e.Graphics.DrawString("GameOver...", font2, br2, 160, 200);
                    font2 = new Font("MS UI Gothic", 40);
                    e.Graphics.DrawString("Enter:Retry Space:End(to title)", font2, br2, 100, 400);
                    break;
                    
            }
        }

        private void commonPrinterOnGaming(PaintEventArgs e)
        {

            bg.ImgPainter(e);
            for (int i = 0; i < blocks.GetLength(0); i++)
            {
                for (int j = 0; j < blocks.GetLength(1); j++)
                {
                    if (blocks[i, j].Life>0) { blocks[i, j].DrawRectObj(e); }

                }
            }

            ball.DrawCircleObj(e);
            bar.DrawRectObj(e);

            SolidBrush brLife = new SolidBrush(Color.Blue);
            Font fontLife = new Font("MS UI Gothic", 30);
            e.Graphics.DrawString("残機:" + String.Format("{0,4}", this.plife), fontLife, brLife, 810, 100);
            e.Graphics.DrawString("得点:" + String.Format("{0,4}", this.totalScore), fontLife, brLife, 810, 150);

        }

        //右上の残機表示
        public void LifeCounter(PaintEventArgs e)
        {
            int count = plife;
            SolidBrush brLife = new SolidBrush(Color.Blue);
            Font fontLife = new Font("MS UI Gothic", 30);
            if(plife < 0)
            {
                count = 0;
            }
            e.Graphics.DrawString("残機:" + count, fontLife, brLife, 820, 100);
        }
    }
  
}

    public class StageConfig
{

    int[][] blockLife;
    BackgroundRect stageBack;
                                          
    

    void rewriteStageInformation()
    {

    }
}
public class SystemConfig
{
    int stageNumber;

}

struct MyPoint//double型Point構造体
    {
        public double X { get; set; }
        public double Y { get; set; }
        
        public void setXY(double x,double y)
    {
        this.X = x;
        this.Y = y;
    }
        public MyPoint(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
    }
    
    //図形の基底クラス
    class Figure
    {
        private int x, y, width, height;

        private Color col = Color.White;

        protected Figure(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
        protected Figure(int x, int y, int width, int height, Color c) : this(x, y, width, height)
        {
            this.col = c;
        }



        public int X
        {
            get { return this.x; }
            protected set { this.x = value; }
        }
        public int Y
        {
            get { return this.y; }
            protected set { this.y = value; }

        }
        public int Width
        {
            get { return width; }

        }
        public int Height
        {
            get { return height; }
        }
        public Color Col
        {
            get { return this.col; }
            set { this.col = value; }
        }

    }
    //長方形基底クラス
    class RectFig : Figure
    {
        public RectFig(int x, int y, int width, int height) : base(x, y, width, height)
        {

        }
        public void DrawRectObj(PaintEventArgs e)
        {
            SolidBrush Br = new SolidBrush(this.Col);
            e.Graphics.FillRectangle(Br, this.X, this.Y, this.Width, this.Height);

        }
    }
    class BackgroundRect : RectFig
    {
        Image image;
      public BackgroundRect(int x, int y, int width, int height) : base(x, y, width, height)
        {

        }
      public BackgroundRect(int x, int y, int width, int height,Image im) : this(x, y, width, height)
        {
            this.image = im; 
        }
      public void ImgPainter(PaintEventArgs e)
        {
            e.Graphics.DrawImage(image, this.X, this.Y,this.Width, this.Height);

        }
    }

    //ブロック
    class Block : RectFig
    {
        int currentLife;
     

        int iniLife;


        public Block(int x, int y, int width, int height, int life) : base(x, y, width, height)
        {
            this.iniLife = life;
            this.currentLife = life;
        }
        
        public int Life
        {
            get { return this.currentLife; }
            private set { this.Life = value; }
       
        }
        
        public void ReduceLife()
        {
            this.currentLife = currentLife - 1;
            
        }
        public void Revival()
        {
            this.Life = this.iniLife;
        }


    }
    //ラケット
    class Bar : RectFig
    {
        int moveWidth;
        int xBeforeOneFrame;

        public Bar(int x, int y, int width, int height, int moveWidth) : base(x, y, width, height)
        {
            this.moveWidth = moveWidth;
        }

        private Boolean movingLeft;
        private Boolean movingRight;
        
        public int XBeforeOneFrame{
            get { return xBeforeOneFrame; }
        }

        public void moveR()
        {
            movingRight = true;
        }
        public void moveL()
        {
            movingLeft = true;
        }
        public void stop()
        {
            movingLeft = false;
            movingRight = false;
            xBeforeOneFrame = X;
        }
        public void Motion()
        {
            if (movingRight)
            {
                xBeforeOneFrame = X;
                X += moveWidth;
            }
            if (movingLeft)
            {
                xBeforeOneFrame = X;
                X -= moveWidth;
            }
        }
        //背景からはみ出ているかチェックする
        public void overBackgroundCheck(BackgroundRect backgroundRect)
        {
            if(this.X + this.Width > backgroundRect.X + backgroundRect.Width)
            {
                this.X = backgroundRect.X + backgroundRect.Width - this.Width + 1;
            }
            if (this.X < backgroundRect.X)
            {
                this.X = backgroundRect.X + 1;

            }
        }
    }
    //円形基底クラス
    class CircleFig : Figure
    {



        protected CircleFig(int x, int y, int width, int height) : base(x, y, width, height)
        {

        }

        public void DrawCircleObj(PaintEventArgs e)
        {
            SolidBrush Br = new SolidBrush(this.Col);
            e.Graphics.FillEllipse(Br, this.X, this.Y, this.Width, this.Height);
        }


    }
    //球
    class Ball : CircleFig
    {
      
        private double dx, dy;


    enum Coners
    {

        Bottom,
        Left,
        Right,
        Top,
        
    }


    public Ball(int x, int y, int width, int height, int dx, int dy) : base(x, y, width, height)
        {
            this.dx = (double)dx;
            this.dy = (double)dy;
        }
        public Ball(int x, int y, int width, int height, int dx, int dy,Bar bar) : base(x, y, width, height)
        {
            this.dx = (double)dx;
            this.dy = (double)dy;

            this.X = bar.X +bar.Width- (bar.Width / 2)-(width/2);
            this.Y = bar.Y - height;
        }
        public double Dx
        {
            get { return this.dx; }
            set { this.dx = value; }
        }
        public double Dy
        {
            get { return this.dy; }
            set { this.dy = value; }
        }

        public void StickingBarMotion(Bar bar)
        {
            X = bar.X + bar.Width/2-this.Width/2;
            Y = bar.Y - this.Height;
        }

        public void Motion()
        {
            X += (int)Dx;
            Y += (int)Dy;
         }
        //座標を外部から変更するための関数
        public void setPoint(int x,int y)
        {
            this.X = x;
            this.Y = y;
        }

        //円周上の8点計算
        MyPoint[] eightPoint = new MyPoint[8];
        /*
        5 6 7 
        3 C 4
        0 1 2
        */
        public MyPoint[] CaliculateEightPoint()
        {

            MyPoint c = new MyPoint((2 * (double)this.X + (double)this.Width) / 2.0, (2 * (double)this.Y + (double)this.Height) / 2.0);


            double r = ((double)this.Width) / 2.0;
            double OppSideLength = r / Math.Sqrt(2.0);


            
            eightPoint[0].setXY(c.X - OppSideLength, c.Y + OppSideLength);
            eightPoint[1].setXY(c.X, this.Y + this.Height);
            eightPoint[2].setXY(c.X + OppSideLength, c.Y + OppSideLength);
            eightPoint[3].setXY(this.X, c.Y);
            eightPoint[4].setXY(this.X + this.Width, c.Y);
            eightPoint[5].setXY(c.X - OppSideLength, c.Y - OppSideLength);
            eightPoint[6].setXY(c.X, this.Y);
            eightPoint[7].setXY(c.X + OppSideLength, c.Y - OppSideLength);

            return eightPoint;
        }
    }



    class Collision
    {
        private Ball ball;
        private Block[,] blocks;
        private Bar bar;
        private BackgroundRect bg;
        private int addScore;
        private int passScore=0;

        private bool checkBarCollisionAble = true;

        private System.Media.SoundPlayer player;


        //角度調節用変数、急になり過ぎないよう
        int Anglecheck = 0;

        public Collision(Ball ball, Block[,] blocks, Bar bar, BackgroundRect bg,int addScore)
        {
            this.ball = ball;
            this.blocks = blocks;
            this.bar = bar;
            this.bg = bg;
            this.addScore = addScore;
            this.player = new System.Media.SoundPlayer(Resources.sound);
        }

        
        enum Coners
        {
          
            Bottom,
            Left ,
            Right,
            Top,
            None
        }
        private void PlaySound()
        {
            player.Play();
        }
        public void ballAndBar()
        {
            if(checkBarCollisionAble)
            {
                int n = CheckEightPoint(this.ball, this.bar);
                ballVecBar(n);
                if (n <= 4)
                {
                    PlaySound();
                    checkBarCollisionAble = false;
                }
                
            }
            
        }
        public void ballAndBlock()
        {
            int n;
            bool checkended = false;
            for (int i = 0; i < blocks.GetLength(0); i++)
            {
                for (int j = 0; j < blocks.GetLength(1); j++)
                {
                    if (blocks[i, j].Life>0)
                    {
                        n = CheckEightPoint(this.ball, blocks[i, j]);
                        ballVec(n);
                        if (n < 10)
                        {
                            PlaySound();
                            blocks[i, j].ReduceLife();
                            passScore += addScore;
                            checkBarCollisionAble = true;
                            checkended = true;

                        }
                    }
                    if (checkended) { break; }
                }
                if (checkended) { break; }
            }
        }
        public int getScore()
    {
        int sub = passScore;
        passScore = 0;
        return sub;
    }
        private void ballVec(int i)
        {
           
            //Conerを使う予定
            switch (i)
            {
                case 0:
                case 2:
                case 5:
                case 7:
                    ball.Dy = -ball.Dy;
                    ball.Dx = -ball.Dx;
                    break;
                case 1:
                case 6:
                    ball.Dy = -ball.Dy;


                    break;
                case 3:
                case 4:
                    ball.Dx = -ball.Dx;
                    break;
                default:
                    break;
            }
        }
        private void ballVecBar(int i)
        {
           //上に同じ

            switch (i)
            {
                case 0:
                case 2:
                    vecterChange();
                    break;
                case 1:
                    vecterChange();
                    break;
                case 3:
                case 4:
                    ball.Dx = -ball.Dx;
                    ball.Dy = -ball.Dy;
                    break;
                default:
                    break;
            }
            
        }
        //円の底がラケットのどこにあたったかで飛び方を変える
        private void vecterChange()
        {
            int ballCenter = (2 * ball.X + ball.Width) / 2;
            if (bar.X+(bar.Width/4)<=ballCenter && ballCenter < bar.X + (3* bar.Width) / 4)
            {
                if (Anglecheck <=2) {
                    ball.Dy = -1.41 * (double)ball.Dy;
                    ball.Dx = 1.41 * ball.Dx / 2.0;
                    Anglecheck++;
                }
                else
                {
                    ball.Dy = -0.99*ball.Dy;
                   
                }
                
                
            }
            else
            {
                if (Anglecheck >= -1)
                {
                    ball.Dy = -1.41 * ball.Dy/ 2.0;
                    ball.Dx = 1.41 * ball.Dx;
                    Anglecheck--;
                }
                else
                {
                    ball.Dy = -1.02*ball.Dy;
                    ball.Dx = 0.98 * ball.Dx;
                }
               
                
            }
        }
        public void ballAndWall()
        {
            if ((ball.X + ball.Width) > bg.X + bg.Width)
            {
                ball.Dx = -ball.Dx;
            }
            else if (ball.X < bg.X)
            {
                ball.Dx = -ball.Dx;
            }

            else if (ball.Y <  bg.Y)
            {
                ball.Dy = -ball.Dy;
                ball.setPoint(ball.X, bg.Y);
                checkBarCollisionAble = true;
            }
            else if ((ball.Y + ball.Height) > bg.Y + bg.Height)
            {
                
            }


        }
        private int CheckEightPoint(Ball ball, RectFig blk)
        {
            int x = 10;
            MyPoint[] p = this.ball.CaliculateEightPoint();

            for (int i = 0; i < p.Length; i++)
            {

                if (CheckCircleInRect(p[i], blk))
                {
                    x = i;
                    break;
                }
            }
            return x;
        }
        private bool CheckCircleInRect(MyPoint p, RectFig blk)
        {
            bool bo = false;
            double x = p.X;
            double y = p.Y;
            //Console.Write("check");
            if ((double)blk.X <= x && (double)blk.X + (double)blk.Width >= x)
            {
                if ((double)blk.Y <= y && y <= (double)blk.Y + (double)blk.Height)
                {

                    bo = true;
                }
            }

            return bo;
        }
        //分割数＝判定回数、より当たり判定を正確にするための関数
        private MyPoint LinerInterlation(MyPoint p, RectFig blk, int divisionNum)
        {

            double iniX = p.X - ball.Dx;
            double iniY = p.Y - ball.Dy;


            double DxDivided = ball.Dx / (double)divisionNum;
            double DyDivided = ball.Dy / (double)divisionNum;



            MyPoint accuratePoint = new MyPoint(iniX, iniY);

            for (int i = 0; i < divisionNum; i++)
            {
                accuratePoint.X += DxDivided;
                accuratePoint.Y += DyDivided;

                if (CheckCircleInRect(accuratePoint, blk))
                {
                    break;
                }
            }
            ball.setPoint((int)accuratePoint.X, (int)accuratePoint.Y);


            return accuratePoint;
        }

    }
   class InitialConfig
{

}
 class Stage
{
    
}
 
       
    class MotionOparater
    {

    }

    //画面表示物（当たり判定なし）
    abstract class OverlayObj
    {
        private int x, y;
     }

    class Sound
    {

    }
   
    class FileManeger
    {

    }

    class DisplayObjects
    {

    }

namespace Physics
{
    
    public class Vector2D
    {
        double x;
        double y;
        public Vector2D(double x,double y)
        {
            this.x = x;
            this.y = y;
        }
       
        public void rotate(double angle)
        {

        }
        public void expand(double coefficient)
        {

        }

        public void changeDirection(double angle)
        {

        }
        public void expandSpecifiedSize(double size)
        {

        }
    }
}


    


