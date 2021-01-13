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
    public partial class FormShift2 : Form
    {
        private int[] normalSizeBinaryA;
        private int[] normalSizeBinaryB;

        private int bitSize;
        private int[] binaryA;
        private int[] binaryB;
        private bool positive;

        string[] displayString = new string[8];
        int lookIndex;//지금 보고있는 페이지 번호, pageIndex는 기록용

        public void FSetData(int bitSize, int[] binaryA, int[] binaryB, bool positive, int[] normalSizeBinaryA, int[] normalSizeBinaryB)//전송 받음
        {
            this.bitSize = bitSize;
            this.binaryA = binaryA;
            this.binaryB = binaryB;
            this.positive = positive;

            this.normalSizeBinaryA = normalSizeBinaryA;
            this.normalSizeBinaryB = normalSizeBinaryB;
        }
        //------------------------------------------------------------------------------------------------------------
        private void FChaingeWindowSize(int x = 0, int y = 0)//창 크기 변경
        {
            this.Size = new Size(x, y);
            if (x == 0 && y == 0)
            {
                switch (bitSize / 2)
                {
                    case 8:
                        this.Size = new Size(300, 1000);
                        break;
                    case 16:
                        this.Size = new Size(450, 1000);
                        break;
                    case 32:
                        this.Size = new Size(850, 1000);
                        break;
                    case 64:
                        this.Size = new Size(1400, 1000);
                        break;
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------
        private string FBinaryToString(int[] binaryArray)//string으로 만듬(사람이 보는 방향으로 역순)
        {
            string tmp1 = String.Join("", binaryArray.Select(p => p.ToString()).ToArray());
            string tmp2 = "";
            for (int i = 0; i < tmp1.Length; i++)
                tmp2 += tmp1[tmp1.Length - i - 1];
            return tmp2;
        }

        private string FBinaryStringToDisplayString(string binaryString)
        {
            string returnString = "";

            for (int i = 0; i < binaryString.Length; i++)
            {
                if (i % 4 == 0)
                    returnString += " ";
                returnString += binaryString[i];
            }
            return returnString;
        }

        private string FBinaryPartSumStringToDisplayString(string binaryString)
        {
            string tmpString = "";
            string returnString = "";
            /*
            for (int i = 0; i < startBitIndex; i++)
                tmpString += "0";
            for (int i = startBitIndex; i < bitSize; i++)
                tmpString += binaryString[i];

            for (int i = 0; i < tmpString.Length; i++)
            {
                if (i % 4 == 0)
                    returnString += " ";
                if (tmpString[i] == '@')
                    returnString += " ";
                else
                    returnString += tmpString[i];
            }
            
            for (int i = 0; i < binaryString.Length; i++)
            {
                if (i % 4 == 0)
                    returnString += " ";
                if (re[i] == '@')
                    returnString += " ";
                else
                    returnString += tmpString[i];
            }

            */
            for (int i = 0; i < binaryString.Length; i++)
            {
                if (i % 4 == 0)
                    returnString += " ";
                returnString += binaryString[i];
            }

            return returnString;
        }
        
        private string FMakeDashLine(int c)//22 --------- 이런 스트링 만들어줌
        {
            string returnString = Convert.ToString(c) + " ";
            for (int i = 0; i < bitSize; i++) returnString += "--";
            return returnString;
        }
        //------------------------------------------------------------------------------------------------------------

        private int[] FBinarySum(int[] binaryA, int[] binaryB)
        {
            int[] returnArray = new int[bitSize];
            int tmpValue = 0;
            int upValue = 0;
            for (int i = 0; i < bitSize; i++)
            {
                tmpValue = upValue + binaryA[i] + binaryB[i];
                if (tmpValue >= 2) upValue = 1;
                else upValue = 0;
                returnArray[i] = tmpValue % 2;
                tmpValue = 0;
            }

            return returnArray;
        }

        private int[] FBinaryRightShift(int [] binaryA)//0101 0000 -> 0010 1000
        {
            //0123 4567
            //1234 567x
            int[] returnArray = new int[bitSize];

            for (int i = 0; i < bitSize - 1; i++)
                returnArray[i] = binaryA[i + 1];

            returnArray[bitSize - 1] = 0;

            return returnArray;
        }


        public FormShift2()
        {
            InitializeComponent();
        }

        private void FormShift2_Load(object sender, EventArgs e)
        {
            FChaingeWindowSize();

            int[] binaryPartSum = new int[bitSize];
            int[] binaryShiftOnly = new int[bitSize];//0000 0000 시프트 할때 보여주기용

            string normalBlinkString = "";
            for(int i=0;i<bitSize/2;i++)//더미에 쓰이는 빈 공간
            {
                if (i % 4 == 0)
                    normalBlinkString += " ";
                normalBlinkString += " ";
            }

            string dummyBinaryAString = FBinaryStringToDisplayString(FBinaryToString(normalSizeBinaryA)) + normalBlinkString;
            string dummyBinaryZeroString = "";
            for (int i = 0; i < bitSize / 2; i++)
            {
                if (i % 4 == 0)
                    dummyBinaryZeroString += " ";
                dummyBinaryZeroString += "0";
            }
            dummyBinaryZeroString += normalBlinkString;


            //--------------------------------------------------------------------------------------------------------
            //0단계
            //--------------------------------------------------------------------------------------------------------
            displayString[0] += "  " + normalBlinkString + FBinaryStringToDisplayString(FBinaryToString(normalSizeBinaryA)) + "\n";
            displayString[0] += " *" + normalBlinkString + FBinaryStringToDisplayString(FBinaryToString(normalSizeBinaryB)) + "\n";
            displayString[0] += FMakeDashLine(0);
            displayString[0] += "\n  " + FBinaryPartSumStringToDisplayString(FBinaryToString(binaryPartSum)) + "\n";
            //--------------------------------------------------------------------------------------------------------
            //N단계
            //--------------------------------------------------------------------------------------------------------
            int pageIndex = 0;//현재 기록중인 페이지
            int lineCount = 0;//원래는 지금 1줄이 들어간 것이 맞으나, 0단계는 이번 첫페이지에서만 표시할거기 때문에 없는 취급함
            int shiftCount = 0;//시프트 횃수
            string lastLine = "";//마지막 줄을 기억함

            for (int i = 0; i < bitSize / 2; i++)
            {
                if (lineCount >= 8)//8줄이 이미 기록되어있다면
                {
                    pageIndex++;//다음 페이지에 기록
                    lineCount = 0;
                    displayString[pageIndex] = lastLine;
                }

                if (binaryB[shiftCount + bitSize / 2] == 1)
                {
                    displayString[pageIndex]+= " +" + dummyBinaryAString + "\n";
                    binaryPartSum = FBinarySum(binaryPartSum, binaryA);
                }
                else
                {
                    displayString[pageIndex] += " +" + dummyBinaryZeroString + "\n";
                }
                displayString[pageIndex] += FMakeDashLine(shiftCount + 1) + "\n";
                displayString[pageIndex] += "  " + FBinaryPartSumStringToDisplayString(FBinaryToString(binaryPartSum)) + "\n";
                binaryPartSum = FBinaryRightShift(binaryPartSum);
                shiftCount++;
                lastLine = ">>" + FBinaryPartSumStringToDisplayString(FBinaryToString(binaryPartSum)) + "\n";
                displayString[pageIndex] += "\n>>" + FBinaryPartSumStringToDisplayString(FBinaryToString(binaryPartSum)) + "\n";
                lineCount++;
            }
            displayString[pageIndex] += "\n " + "END!!!!-----------";
           char pos;
            if (positive)
                pos = '+';
            else
                pos = '-';
            displayString[pageIndex] += "\n " + pos + FBinaryPartSumStringToDisplayString(FBinaryToString(binaryPartSum)) + "\n";
            labelDisplayValue0.Text = displayString[0];
        }

        private void buttonPrev_Click(object sender, EventArgs e)
        {
            if (lookIndex != 0)
                lookIndex--;
            labelDisplayValue0.Text = displayString[lookIndex];
        }
        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (lookIndex != bitSize / 2 / 8 - 1)
                lookIndex++;
            labelDisplayValue0.Text = displayString[lookIndex];
        }


    }
}
