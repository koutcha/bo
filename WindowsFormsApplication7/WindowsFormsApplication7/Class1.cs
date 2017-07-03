using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication7
{

    class KeyManager//キーボードの状態管理のみ行う（押されているかどうか),キー変更時に変数いじらないといけないので削除する予定
    {

        private bool onEnter;
        private bool onRight;
        private bool onLeft;
        private bool onUp;
        private bool onSpace;

        KeyRegistry keyRegistry;

        public bool OnEnter
        {
            get { return this.onEnter; }
        }
        public bool OnRight
        {
            get { return this.onRight; }
        }
        public bool OnLeft
        {
            get { return this.onLeft; }
        }
        public bool OnUp
        {
            get { return this.onUp; }
        }
        public bool OnSpace
        {
            get { return this.onSpace; }
        }
        public void onKeyCheck(KeyEventArgs e)
        {

            switch (e.KeyCode)
            {
                case Keys.Enter:
                    this.onEnter = true;
                    break;
                case Keys.Left:
                    this.onLeft = true;
                    break;
                case Keys.Right:
                    this.onRight = true;
                    break;
                case Keys.Up:
                    this.onUp = true;
                    break;
                case Keys.Space:
                    this.onSpace = true;
                    break;
                default:
                    break;
            }
        }
        public void releaseKeyCheck(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    this.onEnter = false;

                    break;
                case Keys.Left:
                    this.onLeft = false;

                    break;
                case Keys.Right:
                    this.onRight = false;

                    break;
                case Keys.Up:
                    this.onUp = false;
                    break;
                case Keys.Space:
                    this.onSpace = false;
                    break;
                default:
                    break;
            }
        }

    }
    /*使用キーの登録と状態を返すクラス*/
    class KeyRegistry
    {
        public enum KeysState
        {
            NotRegisted,
            Unavailable,
            Onkey,
            Offkey
        }



        private Hashtable keysStatusTable = new Hashtable();




        public KeyRegistry(Keys[] useKeyCodeArray)
        {

            for (int i = 0; i < useKeyCodeArray.Length; i++)
            {
                keysStatusTable.Add(useKeyCodeArray[i], KeysState.Offkey);
            }
        }


        public KeysState getCurrentKeyStatus(Keys keyCode)
        {
            KeysState currentKeyStatus = KeysState.NotRegisted;

            if (keysStatusTable.Contains(keyCode))
            {
                currentKeyStatus = (KeysState)keysStatusTable[keyCode];
            }

            return currentKeyStatus;
        }


        private void setCurrentKeyStatus(Keys keyCode, KeysState currentKeyState)
        {

            if (keysStatusTable.Contains(keyCode))
            {
                keysStatusTable[keyCode] = currentKeyState;
            }
            else
            {
                //erorr: this key is not registed(出力
            }


        }

        public void KeyDownCatcher(KeyEventArgs e)
        {
            setCurrentKeyStatus(e.KeyCode, KeysState.Onkey);

        }
        public void KeyUpCatcher(KeyEventArgs e)
        {
            setCurrentKeyStatus(e.KeyCode, KeysState.Offkey);
        }
    }
    class KeyManage
    {


        private int[] keysFrame;
        private Keys[] keyCodes;
        private KeyRegistry keyResistry;



        public KeyManage(Keys[] useKeyCodeArray)
        {
            keyResistry = new KeyRegistry(useKeyCodeArray);
            keyCodes = useKeyCodeArray;
            keysFrame = new int[useKeyCodeArray.Length];


        }

        public void UpdateKeyFrames()
        {

            for (int i = 0; i < keyCodes.Length; i++)
            {
                if (keyResistry.getCurrentKeyStatus(keyCodes[i]) == KeyRegistry.KeysState.Onkey)
                {
                    keysFrame[i]++;
                }
                else
                {
                    keysFrame[i] = 0;
                }
            }
        }

        public int getKeysDownFrame(Keys key)
        {
            int x = 0;

            for (int i = 0; i < keyCodes.Length; i++)
            {
                if (keyCodes[i] == key)
                {
                    x = keysFrame[i];
                }
            }


            return x;
        }
        public void KeyDownCatcher(KeyEventArgs e)
        {
            this.keyResistry.KeyDownCatcher(e);
        }
        public void KeyUpCatcher(KeyEventArgs e)
        {
            this.keyResistry.KeyUpCatcher(e);
        }

    }
}
