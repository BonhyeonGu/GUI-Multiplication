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
    public partial class FormBooth : Form
    {
        private int bitSize;
        private int[] binaryA;
        private int[] binaryB;

        string[] displayString = new string[8];
        int lookIndex;//지금 보고있는 페이지 번호, pageIndex는 기록용

        public void FSetData(int bitSize, int[] binaryA, int[] binaryB)//전송 받음
        {
            this.bitSize = bitSize;
            this.binaryA = binaryA;
            this.binaryB = binaryB;
        }

        public FormBooth()
        {
            InitializeComponent();
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
                        this.Size = new Size(300, 750);
                        break;
                    case 16:
                        this.Size = new Size(450, 750);
                        break;
                    case 32:
                        this.Size = new Size(850, 750);
                        break;
                    case 64:
                        this.Size = new Size(1400, 750);
                        break;
                }
            }
        }

        //------------------------------------------------------------------------------------------------------------

        private string FBinaryToString(int[] binaryArray)//string으로 만듬(사람이 보는 방향으로 역순)
        {
            string tmp1 = String.Join("", binaryArray.Select(p => p.ToString()).ToArray());
            string tmp2 = "";
            for(int i = 0;i<tmp1.Length;i++)
                tmp2 += tmp1[tmp1.Length - i - 1];
            return tmp2;
        }

        private string FBinaryStringToDisplayString(string binaryString, int startBitIndex)
        {
            string returnString1 = "";
            string returnString2 = "";
            
            for (int i = 0; i < bitSize / 2 - startBitIndex; i++)
                returnString1 += "@";
            for (int i = bitSize / 2 - startBitIndex; i < bitSize - startBitIndex; i++)
                returnString1 += binaryString[i];
            for (int i = bitSize - startBitIndex; i < bitSize; i++)
                returnString1 += "@";

            int index1 = 0, index2 = bitSize / 2 - startBitIndex;
            while (index1<returnString1.Length)
            {
                if (index1 != 0 && index1 % 4 == 0)
                    returnString2 += " ";
                if (returnString1[index1++] != '@')
                {
                    returnString2 += returnString1[index2++];
                }
                else
                    returnString2 += " ";//!
            }

            return returnString2;
        }//사람이 보기 편하게 바꾸어줌

        private string FBinaryPartSumStringToDisplayString(string binaryPartSumString)
        {
            string returnString = "";

            for (int i = 0; i < binaryPartSumString.Length; i++)
            {
                if (i != 0 && i % 4 == 0)
                    returnString += " ";
                returnString += binaryPartSumString[i];
            }

            return returnString;
        }//오직 부분합부분만 사람이 보기 편하게..

        private string FMakeDashLine(int c)//22 --------- 이런 스트링 만들어줌
        {
            string returnString = Convert.ToString(c) + " ";
            for (int i = 0; i < bitSize; i++) returnString += "--";
            return returnString;
        }
        
        
        //------------------------------------------------------------------------------------------------------------
        private int FNumBInterpret(int[] binaryB, int startBitIndex)//가하는 수 두 비트를 명령어로 치환하는 함수
        {
            int nextBitIndex = startBitIndex + 1;
            if (binaryB[nextBitIndex] == binaryB[startBitIndex])
                return 0;//무시
            else if (binaryB[nextBitIndex] == 0)
                return 1;//더해라
            else
                return 2;//빼라
        }
        
        private int[] FBinarySum(int[] binaryA, int[] binaryB)
        {
            int[] returnArray = new int[bitSize];
            int tmpValue = 0;
            int upValue = 0;
            for (int i = 0; i < bitSize; i++)
            {
                tmpValue = upValue + binaryA[i] + binaryB[i];
                if (tmpValue >= 2)  upValue = 1;
                else   upValue = 0;
                returnArray[i] = tmpValue % 2;
                tmpValue = 0;
            }

            return returnArray;
        }

        private int[] FBinaryImsum(int[] binaryA, int[] binaryB, int startBitIndex)
        {
            int[] returnArray = new int[bitSize];
            if (binaryB[bitSize - 1] == 0)//가하는 수가 양수라면
            {
                for (int i = startBitIndex; i < bitSize; i++)//0과 1을 뒤집고
                {
                    if (binaryB[i] == 0)
                        returnArray[i] = 1;
                    else
                        returnArray[i] = 0;
                }
                for (int i = startBitIndex; i < bitSize; i++)//1더함//!!!bitSize+startBitIndex
                {
                    if (returnArray[i] == 0)
                    {
                        returnArray[i] = 1;
                        break;
                    }
                    else
                        returnArray[i] = 0;
                }
            }
            else//가하는 수가 음수라면
            {
                for (int i = startBitIndex; i < bitSize; i++)//1빼고//!!!bitSize+startBitIndex
                {
                    if (binaryB[i] == 1)
                    {
                        returnArray[i] = 0;
                        for (int j = i + 1; j < bitSize; j++)
                            returnArray[j] = binaryB[j];
                        break;
                    }
                    else
                        returnArray[i] = 1;
                }
                for (int i = startBitIndex; i < bitSize; i++)//0과 1을 뒤집음
                {
                    if (returnArray[i] == 0)
                        returnArray[i] = 1;
                    else
                        returnArray[i] = 0;
                }
            }

            return FBinarySum(binaryA, returnArray);//그리고 더함
        }

        private int[] FBinaryLeftShift(int[] binaryA, int startBitIndex)
        {
            int[] returnArray = new int[bitSize];
            int tmp;

            for (int i = 0; i < startBitIndex + 1; i++)//내용 앞과 한칸 더를 0으로 채움
                returnArray[i] = 0;
            for (int i = startBitIndex; i < (startBitIndex + bitSize / 2); i++)//내용은 한칸씩 뒤로
                returnArray[i + 1] = binaryA[i];
            tmp = binaryA[startBitIndex + bitSize / 2 - 1];//최고 자리수 (0또는 1)로 뒤를 채운다.
            for (int i = startBitIndex + bitSize / 2 + 1; i < bitSize; i++)
                returnArray[i] = tmp;

            return returnArray;
        }

        //------------------------------------------------------------------------------------------------------------

        private void FormBooth_Load(object sender, EventArgs e)
        {
            FChaingeWindowSize();
            int[] binaryPartSum = new int[bitSize];//부분합
            int[] binaryShiftOnly = new int[bitSize];//0000 0000 시프트 할때 보여주기용
            //--------------------------------------------------------------------------------------------------------
            //0단계
            //--------------------------------------------------------------------------------------------------------
            displayString[0] += "  " + FBinaryStringToDisplayString(FBinaryToString(binaryA), 0) + "\n";
            displayString[0] += " *" + FBinaryStringToDisplayString(FBinaryToString(binaryB), 0) + "\n";
            displayString[0] += FMakeDashLine(0);
            displayString[0] += "\n  " + FBinaryPartSumStringToDisplayString(FBinaryToString(binaryPartSum)) + "\n";
            //--------------------------------------------------------------------------------------------------------
            //1단계 추가 1비트 0과 확인하는 단계
            //--------------------------------------------------------------------------------------------------------
            if (binaryB[0] == 0)//00
                displayString[0] += " +" + FBinaryStringToDisplayString(FBinaryToString(binaryShiftOnly), 0) + "\n";
            else//10
            {
                displayString[0] += " -" + FBinaryStringToDisplayString(FBinaryToString(binaryA), 0) + "\n";
                binaryPartSum = FBinaryImsum(binaryPartSum, binaryA, 0);
            }
            displayString[0] += FMakeDashLine(1);
            displayString[0] += "\n  " + FBinaryPartSumStringToDisplayString(FBinaryToString(binaryPartSum)) + "\n";
            binaryA = FBinaryLeftShift(binaryA, 0);
            binaryShiftOnly = FBinaryLeftShift(binaryShiftOnly, 0);

            
            //--------------------------------------------------------------------------------------------------------
            //N단계
            //--------------------------------------------------------------------------------------------------------
            int pageIndex = 0;//현재 기록중인 페이지
            int lineCount = 1;//원래는 지금 2줄이 들어간 것이 맞으나, 0단계는 이번 첫페이지에서만 표시할거기 때문에 없는 취급함
            int shiftCount = 1;//시프트 횃수
            string algoCount;

            for (int i = 0; i < bitSize / 2 - 1; i++) 
            {
                if (lineCount >= 8)//8줄이 이미 기록되어있다면
                {
                    pageIndex++;//다음 페이지에 기록
                    lineCount = 0;
                }

                if (pageIndex != 0 && lineCount == 0)
                    displayString[pageIndex] += "  " + FBinaryPartSumStringToDisplayString(FBinaryToString(binaryPartSum)) + "\n";

                switch (FNumBInterpret(binaryB,i))
                {
                    case 0:
                        displayString[pageIndex] += " +" + FBinaryStringToDisplayString(FBinaryToString(binaryShiftOnly), shiftCount) + "\n";
                        break;
                    case 1:
                        displayString[pageIndex] += " +" + FBinaryStringToDisplayString(FBinaryToString(binaryA), shiftCount) + "\n";
                        binaryPartSum = FBinarySum(binaryPartSum, binaryA);
                        break;
                    case 2:
                        displayString[pageIndex] += " -" + FBinaryStringToDisplayString(FBinaryToString(binaryA), shiftCount) + "\n";
                        binaryPartSum = FBinaryImsum(binaryPartSum, binaryA, shiftCount);
                        break;
                }
                displayString[pageIndex] += FMakeDashLine(shiftCount + 1);
                displayString[pageIndex] += "\n  " + FBinaryPartSumStringToDisplayString(FBinaryToString(binaryPartSum)) + "\n";
                binaryA = FBinaryLeftShift(binaryA, shiftCount);
                binaryShiftOnly = FBinaryLeftShift(binaryShiftOnly, shiftCount);

                shiftCount++;
                lineCount++;//한 줄 기록 완료
            }
            displayString[pageIndex] += "\n " + "END!!!!-----------";
            displayString[pageIndex] += "\n " + FBinaryPartSumStringToDisplayString(FBinaryToString(binaryPartSum)) + "\n";

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
