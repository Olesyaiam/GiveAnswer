using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GiveAnswer
{
    public partial class Form1 : Form
    {
        private const string AppName = "Give answer";
        private readonly string ConfigPath = $"{Environment.CurrentDirectory}\\Config.json";
        private string[] _predictions;
        private Random _random = new Random();

        public Form1() // в конструкторе не может работать метод Close(), т.к. форма еще не загрузилась
        {
            InitializeComponent();
            Text = AppName;
        }

        private void Form1_Load(object sender, EventArgs e) // выполняется метод когда форма полностью загрузилась на экран
        {
            try
            {
                string data = File.ReadAllText(ConfigPath);

                // в поле сохраняем результат конвертации в массив строк из переменной data
                _predictions = JsonConvert.DeserializeObject<string[]>(data);
            }
            catch (Exception)
            {
                MessageBox.Show($"Файла {ConfigPath} не существует");
                Close();
            }
        }

        private async void buttonAnswer_Click(object sender, EventArgs e)
        {
            // делаем false чтобы не было возможности несколько раз запускать
            buttonAnswer.Enabled = false;

            // для избежания фризов блокировки юзер интерфейса, сделаем цикл фор в отдельном потоке
            // реализуем с помощью Task (Одна задача может запускать другую - вложенную задачу.
            // При этом эти задачи выполняются независимо друг от друга)
            await Task.Run(() => // с помощью await ждем 
            {
                // Заполнение прогрессбара от 0 до 100
                for (int i = 1; i <= 120; i++)
                {
                    // метод Invoke полезен в случаях, когда необходимо работать с контролом
                    // из других потоков
                    Invoke(new Action(() =>
                    {
                        double procent = Math.Round((i - 10) / 1.1);

                        if (i < 10)
                        {
                            procent = 0;
                        }

                        Text = $"{procent}%";

                        progressBar1.Value = i >= 100 ? 100 : i; ;

                    }));
                    // обращаемся к классу thread вызываем метод sleep 

                    Thread.Sleep(20);
                }

                Thread.Sleep(100);

                Invoke(new Action(() =>
                {
                    // генерируем число от 0 до размера массива (кол-ва предсказаний)
                    int index = _random.Next(0, _predictions.Length);


                    MessageBox.Show(_predictions[index]);
                    buttonAnswer.Enabled = true;
                }));

            });



            progressBar1.Value = 0;
            Text = AppName;
        }


    }
}
