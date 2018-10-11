using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace UML2Netbeans
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        String imagePath = "";
        String netbeansPath = "";
        String jsFilePath = "";
        bool screenshotTaken = false;
        private async void button1_ClickAsync(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            comboBox1.Enabled = false;
            OCR ocr = new OCR();
            String textResult = await ocr.convertToTextAsync(imagePath);

            String finalText = optimizeText(textResult);

            String className = getClassName(finalText);
            finalText = finalText.Replace(className + System.Environment.NewLine, "");



            String[] publicAttribute = getPublicAttribute(finalText);
            String[] privateAttribute = getPrivateAttribute(finalText);

            String[] publicMethoden = getPublicMethoden(finalText);
            String[] privateMethoden = getPrivateMethoden(finalText);


            //JS Datei für alle PUBLIC ATTRIBUTE
            for (int i = 0; i < publicAttribute.Length; i++)
            {

                int typeOfAttribut = getTypeOfAttribut(publicAttribute[i]);
                //0 Steht für die Art( 0 = Attribut, 1 = Methode ) 
                String genCode = generateCode(publicAttribute[i], 0, typeOfAttribut, "public");
                jsFilePath = writeToJSFile(genCode, className);
            }

            //JS Datei für alle PRIVATE ATTRIBUTE
            for (int i = 0; i < privateAttribute.Length; i++)
            {

                int typeOfAttribut = getTypeOfAttribut(privateAttribute[i]);
                //0 Steht für die Art( 0 = Attribut, 1 = Methode ) 
                String genCode = generateCode(privateAttribute[i], 0, typeOfAttribut, "private");
                jsFilePath = writeToJSFile(genCode, className);
            }

            //JS Datei für alle PUBLIC METHODEN
            for (int i = 0; i < publicMethoden.Length; i++)
            {
                int typeOfAttribut = getTypeOfAttribut(publicMethoden[i]);
                //0 Steht für die Art( 0 = Attribut, 1 = Methode ) 
                String genCode = generateCode(publicMethoden[i], 1, typeOfAttribut, "public");

                jsFilePath = writeToJSFile(genCode, className);
            }



            //JS Datei für alle PRIVATE METHODEN
            for (int i = 0; i < privateMethoden.Length; i++)
            {
                int typeOfAttribut = getTypeOfAttribut(privateMethoden[i]);
                //0 Steht für die Art( 0 = Attribut, 1 = Methode ) 
                String genCode = generateCode(privateMethoden[i], 1, typeOfAttribut, "private");

                jsFilePath = writeToJSFile(genCode, className);
            }

            cleanJSFileUp(jsFilePath, className);
            MessageBox.Show("done");

            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            comboBox1.Enabled = true;

        }


        public void cleanJSFileUp(String jsFilePath, String className)
        {
            String jsPackage = "package " + comboBox1.Text + ";" + System.Environment.NewLine + System.Environment.NewLine;
            String initClass = "public class " + className + " {" + System.Environment.NewLine + System.Environment.NewLine;
            String rawJs = File.ReadAllText(jsFilePath);
            String cleanJs = jsPackage + initClass + rawJs + System.Environment.NewLine + "}";
            File.WriteAllText(jsFilePath, cleanJs);
            //Test123
        }

        public String writeToJSFile(String genCode, String className)
        {
            String package = "";
            if (netbeansPath == "")
            {
                MessageBox.Show("Bitte wählen Sie ein leeres Netbeans-Projekt aus!");
                System.Environment.Exit(0);
            }
            else
            {
                if (comboBox1.Text == "")
                {
                    MessageBox.Show("Bitte geben Sie den Namen ihres Packetes an.");
                    System.Environment.Exit(0);
                }
                else
                {
                    package = comboBox1.Text.Replace(".", "\\");
                    if (Directory.Exists(netbeansPath + "\\src\\" + package))
                    {
                        File.AppendAllText(netbeansPath + "\\src\\" + package + "\\" + className + ".java", genCode + System.Environment.NewLine);
                    }
                    else
                    {
                        Directory.CreateDirectory(netbeansPath + "\\src\\" + package);
                    }


                }



            }
            String jsFilePath = netbeansPath + "\\src\\" + package + "\\" + className + ".java";
            return jsFilePath;
            //Den code in die richtige JS Datei in den richtigen Ordner schreiben.
        }


        //                          Text von UML      0 = Attribut     0 = Normal        public
        //                                            1 = Methode      1 = Static        private
        public String generateCode(String toConvert, int generalType, int specificType, String callType)
        {
            String code = "";
            if (generalType == 0) //Attribut
            {
                String attributdataType = getDataTypeFromAttribut(toConvert);
                String attributName = filterAttributName(toConvert);

                if (specificType == 1) //Static
                {
                    String value = getValue(toConvert);
                    code = callType + " " + "static " + attributdataType + " " + attributName + " = " + value + ";";

                }
                else //Normales Attribut
                {
                    code = callType + " " + attributdataType + " " + attributName + ";";
                }


            }
            else if (generalType == 1) //Methode
            {

                //Code für eine Methode generieren.
                //Public oder Private ist als String übergeben
                String methodenDataType = getMethodenDataType(toConvert);
                String methodenName = getMethodenName(toConvert);
                String parameter = getMethodenParameter(toConvert);
                String returnParameter = getMethodenReturnParameter(methodenDataType);
                //MethodenName und Parameter abrufen
                code = callType + " " + methodenDataType + " " + methodenName + "(" + parameter + ")" + " " + "{" + System.Environment.NewLine + System.Environment.NewLine + returnParameter + System.Environment.NewLine + "}" + System.Environment.NewLine;

            }

            return code;
        }

        public String getMethodenReturnParameter(String methodenDataType)
        {
            String res = "";
            switch (methodenDataType.Replace(" ", ""))
            {
                case "void":
                    res = "";
                    break;
                case "double":
                    res = "return 0;";
                    break;
                case "String":
                    res = "return null;";
                    break;
                case "int":
                    res = "return 0;";
                    break;
                case "boolean":
                    res = "return false;";
                    break;
                case "":
                    res = "";
                    break;
                default:
                    res = "return null;";
                    break;
            }

            return System.Environment.NewLine + res;

        }

        public String getMethodenParameter(String toConvert)
        {
            string[] split1 = toConvert.Split(new[] { "(", ")" }, StringSplitOptions.None);

            String[] split2 = split1[1].Split(',');
            String[] split3;
            String combo = "";

            if (split1[1].Equals(""))
            {
                combo = "";
            }
            else
            {
                try
                {
                    for (int i = 0; i < split2.Length; i++)
                    {
                        split3 = split2[i].Split(':');
                        split3[1] = split3[1].Replace(" ", "");
                        split3[0] = split3[0].Replace(" ", "");
                        combo += split3[1];
                        combo += " ";
                        combo += split3[0];
                        combo += ", ";
                        combo = combo.Replace("»", "");
                    }
                }
                catch
                {
                    MessageBox.Show("Bist du sicher, dass das ein UML Diagramm ist?");
                }

                combo = combo.Remove(combo.Length - 2); //Letzte zwei Zeichen entfernen.
            }


            return combo;
        }

        public String getMethodenName(String toConvert)
        {
            String final = "";
            String[] split1 = toConvert.Split('(');
            if (split1[0].Contains("+"))
            {
                final = split1[0].Replace("+", "");
            }
            else if (split1[0].Contains("-"))
            {
                final = split1[0].Replace("-", "");
            }
            return final;
        }

        public String getMethodenDataType(String toConvert)
        {
            String[] split2;
            String[] split1 = toConvert.Split(')');
            String dataType = "";
            if (split1[1].Contains(":"))
            {
                split2 = split1[1].Split(':');
                dataType = split2[1].Replace(" ", "");
            }
            else
            {
                //It's a constructor! NOOOOOOOO
                dataType = "";
            }

            return dataType;
        }

        public String filterAttributName(String toConvert)
        {
            string[] split1 = toConvert.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
            String res = split1[0].Replace("+", "");
            res = res.Replace("-", "");
            char lastChar = res[res.Length - 1];
            if (res[res.Length - 1] == ' ')
            {
                res = res.Remove(res.Length - 1);
            }

            res = res.Replace(" ", "_");
            return res;
        }

        public String getValue(String toConvert)
        {
            String res = "";
            string[] split1 = toConvert.Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries);

            res = split1[1].Replace(" ", "");

            return res;
        }


        public String getDataTypeFromAttribut(String toConvert)
        {
            String dataType = null;
            try
            {

                string[] split2;
                string[] split1 = toConvert.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                if (split1[1].Contains("="))
                {
                    split2 = split1[1].Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    dataType = split2[0].Replace(" ", "");
                }
                else
                {
                    dataType = split1[1].Replace(" ", "");
                }
            }
            catch
            {

            }

            return dataType;
        }





        public String optimizeText(String textResult)
        {
            string x = textResult;

            StringBuilder builder = new StringBuilder(x);
            builder.Replace("int = O", "int = 0");
            builder.Replace("int= O", "int= 0");
            builder.Replace("int=O", "int=0");
            builder.Replace("double-", "double =");
            builder.Replace("int;", "int");
            builder.Replace("1ass", "lass");


            string y = builder.ToString();


            return y;
        }

        public int getTypeOfAttribut(String attribut)
        {
            //0 = Normal
            //1 = STATIC (Konstant)


            int res = 0;
            String[] split1 = attribut.Split(':');
            split1[0].Replace(" ", "");

            if ((split1[0].ToUpper()).Equals(split1[0]))
            {
                res = 1;
            }

            return res;
        }

        public String getClassName(String finalText)
        {


            String res = null;
            string[] split1 = finalText.Split(new[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            if (!split1[0].Contains("+") && !split1[0].Contains("-"))
            {
                res = split1[0].Replace(" ", "");
            }
            else
            {
                res = "Error at getClassName";
            }

            return res;
        }

        public String[] getPublicAttribute(String finalText)
        {
            List<string> allPublicAttribute = new List<string>();
            string[] split1 = finalText.Split(new[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < split1.Length; i++)
            {
                if (!split1[i].Contains("(") && split1[i].Contains("+"))
                {
                    allPublicAttribute.Add(split1[i]);
                }
            }

            return allPublicAttribute.ToArray();
        }

        public String[] getPrivateAttribute(String finalText)
        {
            List<string> allPrivateAttribute = new List<string>();
            string[] split1 = finalText.Split(new[] { System.Environment.NewLine }, StringSplitOptions.None);
            for (int i = 0; i < split1.Length; i++)
            {
                if (!split1[i].Contains("(") && split1[i].Contains("-"))
                {
                    allPrivateAttribute.Add(split1[i]);
                }
            }
            return allPrivateAttribute.ToArray();
        }

        public String[] getPublicMethoden(String finalText)
        {
            List<string> allPublicMethoden = new List<string>();
            string[] split1 = finalText.Split(new[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < split1.Length; i++)
            {
                if (split1[i].Contains("(") && split1[i].Contains("+"))
                {
                    allPublicMethoden.Add(split1[i]);
                }
            }

            return allPublicMethoden.ToArray();
        }

        public String[] getPrivateMethoden(String finalText)
        {
            List<string> allPrivateMethoden = new List<string>();
            string[] split1 = finalText.Split(new[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < split1.Length; i++)
            {
                if (split1[i].Contains("(") && split1[i].Contains("-"))
                {
                    allPrivateMethoden.Add(split1[i]);
                }
            }

            return allPrivateMethoden.ToArray();
        }



        private void button3_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            netbeansPath = folderBrowserDialog1.SelectedPath;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.youtube.com/channel/UCXhwSGa3-k86JMN_F1NV47Q");
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            imagePath = openFileDialog1.FileName;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //Screenshot machen und Pfad des Bildes abspeichern
            this.WindowState = FormWindowState.Minimized;
            KeyboardSend keyboardSend = new KeyboardSend();

            keyboardSend.KeyDown(Keys.LWin);
            keyboardSend.KeyDown(Keys.LShiftKey);
            keyboardSend.KeyDown(Keys.S);

            keyboardSend.KeyUp(Keys.LWin);
            keyboardSend.KeyUp(Keys.LShiftKey);
            keyboardSend.KeyUp(Keys.S);

            screenshotTaken = true;

        }

        private void onWindowActivated(object sender, EventArgs e)
        {
            if (screenshotTaken)
            {
                try
                {
                    Clipboard.GetImage().Save(System.IO.Path.GetTempPath() + "uml2NetbeansTempImage.jpg");
                    imagePath = System.IO.Path.GetTempPath() + "uml2NetbeansTempImage.jpg";
                    screenshotTaken = false;
                }
                catch
                {
                    MessageBox.Show("Error converting clipboard Data to Image");
                    screenshotTaken = false;
                }
            }
            if (!imagePath.Equals(""))
            {
                labelImageLoaded.Text = "Bild erfolgreich geladen";
                labelImageLoaded.ForeColor = Color.FromArgb(0x56B548); //Grün
            } else
            {
                labelImageLoaded.Text = "Bild konnte nicht geladen werden";
                labelImageLoaded.ForeColor = Color.FromArgb(0xC62D32); //Rot

            }

        }

        private void onInputBoxClick(object sender, EventArgs e)
        {
            try
            {
                Package package = new Package();
                String[] packageArray = package.getPackage(netbeansPath);

                foreach (var item in packageArray)
                {
                    string[] splitTemp = item.Split(new[] { "src\\" }, StringSplitOptions.None);

                    if (!comboBox1.Items.Contains(splitTemp[1].Replace("\\", ".")))
                    {
                        comboBox1.Items.Add(splitTemp[1].Replace("\\", "."));
                    }

                }

                comboBox1.SelectedIndex = 0;

            }
            catch
            {
                //Kein oder flasches Projekt ausgewählt
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                File.Delete(System.IO.Path.GetTempPath() + "uml2NetbeansTempImage.jpg");
            }


            if (e.CloseReason == CloseReason.WindowsShutDown)
            {
                File.Delete(System.IO.Path.GetTempPath() + "uml2NetbeansTempImage.jpg");
            }

            if (e.CloseReason == CloseReason.TaskManagerClosing)
            {
                File.Delete(System.IO.Path.GetTempPath() + "uml2NetbeansTempImage.jpg");
            }

        }

    }
}