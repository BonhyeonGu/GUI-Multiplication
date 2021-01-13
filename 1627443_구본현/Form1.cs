using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _1627443_구본현
{
    public partial class Form1 : Form
    {

        private int bitSize;
        private int algo;
        private class numset
        {
            public bool questionMeetMuliply;

            public bool positiveA;
            public ulong numA;

            public bool positiveB;
            public ulong numB;

            public int errorCode;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void num1_Click(object sender, EventArgs e)
        {
            Button press = (Button)sender;
            textBox1.Text += press.Text;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBoxBit.SelectedItem = "16";
            comboBoxAlgo.SelectedItem = "Booth";
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            numset inputSet = FSplitInput(textBox1.Text);//입력받은걸 쪼갬

            switch(inputSet.errorCode)
            {
                case 1:
                    MessageBox.Show("곱하기가 들어가 있지 않습니다.");
                    return;
                case 2:
                    MessageBox.Show("최대값을 벗어났습니다.");
                    return;
            }
            
            if (algo == 1)//Booth
            {
                FormBooth newForm;
                newForm = new FormBooth();

                int[] binaryA = FChangeBinary(inputSet.numA, inputSet.positiveA);
                int[] binaryB = FChangeBinary(inputSet.numB, inputSet.positiveB);

                //두배로 만듬
                binaryA = FBinaryBoothSizeUp(binaryA, bitSize * 2);
                binaryB = FBinaryBoothSizeUp(binaryB, bitSize * 2);

                newForm.FSetData(bitSize * 2, binaryA, binaryB);
                newForm.Show();//왜 이걸 연달아 적어주지 않으면 오류인가?
            }
            else if (algo == 2)//Shift2
            {
                bool positive;
                if (inputSet.positiveA == inputSet.positiveB)
                    positive = true;
                else positive = false;

                int[] binaryA = FChangeBinary(inputSet.numA, true);//모두 다 양수라고 치고 시작함
                int[] binaryB = FChangeBinary(inputSet.numB, true);

                int[] largeBinaryA = FBinaryShift2SizeUp(binaryA, bitSize * 2);
                int[] largeBinaryB = FBinaryShift2SizeUp(binaryB, bitSize * 2);

                FormShift2 newForm;
                newForm = new FormShift2();
                newForm.FSetData(bitSize * 2, largeBinaryA, largeBinaryB, positive, binaryA, binaryB);
                newForm.Show();//왜 이걸 연달아 적어주지 않으면 오류인가?
            }

        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void comboBoxBit_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            bitSize = Convert.ToInt32(cb.Items[cb.SelectedIndex].ToString());
        }
        private void comboBoxAlgo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;

            if (cb.Items[cb.SelectedIndex].ToString().Equals("Booth"))
                algo = 1;
            else
                algo = 2;
        }
        //---

        private int[] FChangeBinary(ulong before, bool positive)//10진수를 2진수로 변환시켜줌
        {
            int[] input = new int[bitSize];
            int index = 0;
            input = Enumerable.Repeat(0, bitSize).ToArray();//초기화
            while (before != 0)//2진수 변환
            {
                if (before % 2 == 1)
                    input[index++] = 1;
                else input[index++] = 0;
                before /= 2;
            }
            if (!positive)//음수였는가?
            {
                for (int i = 0; i < bitSize; i++)//0과 1을 바꿈 : 1의 보수
                {
                    if (input[i] == 0)
                        input[i] = 1;
                    else
                        input[i] = 0;
                }

                for (int i = 0; i < index; i++)
                {
                    if (input[i] == 0)
                    {
                        input[i] = 1;
                        break;
                    }
                    else
                    {
                        input[i] = 0;//??????
                    }
                }
            }
            return input;
        }

        //1 곱하기 없음
        //2 최대값 초과
        private numset FSplitInput(string input)//오류를 체크하며 입력받은 문자열을 적절히 쪼개고 음수는 음수라고 알림
        {
            numset tmp = new numset();

            tmp.errorCode = 0;
            
            tmp.positiveA = true;
            tmp.positiveB = true;

            string strA = "";
            string strB = "";

            tmp.questionMeetMuliply = false;
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '*')//지금 곱하기를 만났는가?
                {
                    tmp.questionMeetMuliply = true;
                }
                else if (!tmp.questionMeetMuliply)//곱하기를 아직 만나지 못했는가?
                {
                    if (input[i] == '-')//곱하기를 아직 만나지 못했는데 마이너스 기호를 보았는가?
                        tmp.positiveA = false;
                    else if (input[i] != '(' && input[i] != ')')//소괄호 무시하고 숫자값이면..
                        strA += input[i];
                }
                else//곱하기를 이미 만났는가?
                {
                    if (input[i] == '-')
                        tmp.positiveB = false;
                    else if (input[i] != '(' && input[i] != ')')//소괄호 무시하고 숫자값이면..
                        strB += input[i];
                }
            }
            //-------------------------------------------------------------------------------------------
            //에러체크 곱하기의 존재
            //-------------------------------------------------------------------------------------------
            if (!tmp.questionMeetMuliply)
            {
                tmp.errorCode = 1;
                return tmp;
            }
            //-------------------------------------------------------------------------------------------
            tmp.numA = Convert.ToUInt64(strA);
            tmp.numB = Convert.ToUInt64(strB);

            //-------------------------------------------------------------------------------------------
            //에러체크 최대값
            //-------------------------------------------------------------------------------------------
            ulong bitOfMax = 1;
            for (int i = 0; i < bitSize - 1; i++)
                bitOfMax *= 2;
            if (tmp.numA >= bitOfMax || tmp.numB >= bitOfMax)//둘 중 하나라도 최대범위를 벗어났는가?
                tmp.errorCode = 2;
            //-------------------------------------------------------------------------------------------


            return tmp;
        }

        private int[] FBinaryBoothSizeUp(int[] inputArray, int wantSize)//1101 -> 0000 1101
        {
            int[] returnArray = new int[wantSize];
            int i = 0, tmp;
            while (i < inputArray.Length)
            {
                returnArray[i] = inputArray[i];
                i++;
            }
            tmp = returnArray[i - 1];
            while (i < wantSize)
                returnArray[i++] = tmp;
            return returnArray;
        }

        private int[] FBinaryShift2SizeUp(int[] inputArray, int wantSize)//1101 -> 1101 0000
        {
            int[] returnArray = new int[wantSize];

            for (int i = 0; i < wantSize - inputArray.Length; i++)
                returnArray[i] = 0;
            for (int i = 0; i < inputArray.Length; i++)
                returnArray[i + inputArray.Length] = inputArray[i];
            return returnArray;
        }
    }
}
