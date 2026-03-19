using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab09_OOP
{
    // Головний клас форми, який наслідує стандартний клас Form з Windows Forms
    public class MainForm : Form
    {
        // --- Оголошення елементів інтерфейсу (UI) ---
        private Label lblInput;       // Текстовий напис "Коефіцієнт R:"
        private TextBox txtR;         // Поле для введення значення R користувачем
        private Button btnDraw;       // Кнопка для запуску побудови графіка
        private Button btnSave;       // Кнопка для збереження графіка як картинки
        private Label lblScale;       // Текстовий напис "Масштаб:"
        private TrackBar tbScale;     // Повзунок для зміни масштабу відображення
        private PictureBox picGraph;  // Область (полотно), на якій буде малюватися графік

        // --- Змінні для математичних розрахунків та малювання ---
        private double R = 10;        // Початкове значення коефіцієнта астроїди
        private int drawScale = 15;   // Поточний масштаб (скільки пікселів в 1 математичній одиниці)

        // Конструктор форми - викликається при створенні вікна
        public MainForm()
        {
            // Налаштування параметрів головного вікна
            this.Text = "Побудова графіка: Астроїда (Розширена версія)"; // Заголовок вікна
            this.Size = new Size(650, 650);                                // Розмір вікна (ширина, висота)
            this.StartPosition = FormStartPosition.CenterScreen;           // Позиція вікна по центру екрана при запуску

            // Ініціалізація та налаштування елементів керування
            lblInput = new Label() { Text = "Коефіцієнт R:", Location = new Point(10, 15), AutoSize = true };
            txtR = new TextBox() { Location = new Point(100, 12), Width = 60, Text = "10" };
            btnDraw = new Button() { Text = "Побудувати", Location = new Point(170, 10), Width = 90 };
            btnSave = new Button() { Text = "Зберегти", Location = new Point(270, 10), Width = 90 };

            lblScale = new Label() { Text = "Масштаб:", Location = new Point(380, 15), AutoSize = true };

            // Налаштування повзунка масштабу
            tbScale = new TrackBar()
            {
                Location = new Point(440, 10),
                Width = 150,
                Minimum = 5,        // Мінімальний масштаб
                Maximum = 50,       // Максимальний масштаб
                Value = 15,         // Початкове значення (збігається зі змінною drawScale)
                TickFrequency = 5   // Частота поділок на повзунку
            };

            // Налаштування полотна для малювання
            picGraph = new PictureBox()
            {
                Location = new Point(10, 60),
                Size = new Size(610, 530),
                BackColor = Color.White,               // Білий фон
                BorderStyle = BorderStyle.FixedSingle, // Рамка навколо полотна
                // Anchor дозволяє полотну розтягуватися разом зі зміною розміру головного вікна
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            // --- Прив'язка подій до методів-обробників ---
            // Коли відбувається дія (клік, прокрутка), програма викликає відповідний метод
            btnDraw.Click += BtnDraw_Click;
            btnSave.Click += BtnSave_Click;
            tbScale.Scroll += TbScale_Scroll;
            picGraph.Paint += PicGraph_Paint; // Подія Paint викликається щоразу, коли полотно треба перемалювати

            // Додавання створених елементів на форму (без цього вони будуть невидимі)
            this.Controls.Add(lblInput);
            this.Controls.Add(txtR);
            this.Controls.Add(btnDraw);
            this.Controls.Add(btnSave);
            this.Controls.Add(lblScale);
            this.Controls.Add(tbScale);
            this.Controls.Add(picGraph);
        }

        // Обробник події зміни значення повзунка масштабу
        private void TbScale_Scroll(object sender, EventArgs e)
        {
            drawScale = tbScale.Value; // Оновлюємо змінну масштабу значенням з повзунка
            picGraph.Invalidate();     // Invalidate() наказує системі перемалювати PictureBox (викличе подію Paint)
        }

        // Обробник події натискання на кнопку "Побудувати"
        private void BtnDraw_Click(object sender, EventArgs e)
        {
            // Намагаємося перетворити текст з поля txtR у число з крапкою (double)
            if (double.TryParse(txtR.Text, out double inputR))
            {
                R = inputR;            // Якщо успішно - оновлюємо радіус
                picGraph.Invalidate(); // Перемальовуємо графік з новим радіусом
            }
            else
            {
                // Якщо користувач ввів текст замість числа, показуємо вікно з помилкою
                MessageBox.Show("Будь ласка, введіть коректне числове значення для R.", "Помилка вводу");
            }
        }

        // Обробник події натискання на кнопку "Зберегти"
        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Використовуємо SaveFileDialog для вибору місця та формату збереження файлу
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                // Налаштовуємо доступні формати файлів
                sfd.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp";
                sfd.Title = "Зберегти графік як зображення";
                sfd.FileName = "AstroidGraph.png"; // Ім'я файлу за замовчуванням

                // Якщо користувач натиснув "Зберегти" у діалоговому вікні
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    // Створюємо порожнє зображення (Bitmap) з розмірами нашого полотна
                    Bitmap bmp = new Bitmap(picGraph.Width, picGraph.Height);

                    // Копіюємо вміст PictureBox у створений Bitmap
                    picGraph.DrawToBitmap(bmp, new Rectangle(0, 0, picGraph.Width, picGraph.Height));

                    // Визначаємо формат збереження на основі розширення, яке обрав користувач
                    ImageFormat format = ImageFormat.Png;
                    if (sfd.FileName.EndsWith("jpg")) format = ImageFormat.Jpeg;
                    else if (sfd.FileName.EndsWith("bmp")) format = ImageFormat.Bmp;

                    // Зберігаємо файл
                    bmp.Save(sfd.FileName, format);
                    MessageBox.Show("Графік успішно збережено!", "Успіх");
                }
            }
        }

        // Метод-обробник події малювання (Paint). Викликається системою автоматично.
        private void PicGraph_Paint(object sender, PaintEventArgs e)
        {
            // Передаємо об'єкт Graphics (який вміє малювати лінії, текст тощо) у наш власний метод
            DrawGraph(e.Graphics, picGraph.Width, picGraph.Height);
        }

        // Метод, який безпосередньо виконує всю математику і малювання
        private void DrawGraph(Graphics g, int width, int height)
        {
            // Увімкнення згладжування (AntiAlias), щоб лінії не були "драбинкою"
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(Color.White); // Очищення фону білим кольором перед новим малюванням

            // Визначення центру екрана (це буде точка 0,0 нашої системи координат)
            int cx = width / 2;
            int cy = height / 2;

            // --- 1. Малювання осей координат ---
            Pen axisPen = new Pen(Color.Black, 1); // Чорний олівець товщиною 1 піксель
            g.DrawLine(axisPen, 0, cy, width, cy); // Горизонтальна вісь (X)
            g.DrawLine(axisPen, cx, 0, cx, height);// Вертикальна вісь (Y)

            // Налаштування шрифту для написів
            Font font = new Font("Arial", 8);
            Brush brush = Brushes.Black;

            // Підписи осей на кінцях
            g.DrawString("X", font, brush, width - 20, cy - 15);
            g.DrawString("Y", font, brush, cx + 10, 5);

            // --- 2. Малювання поділок та їх підписів на осях ---
            int step = 5; // Крок для поділок (5, 10, 15 і т.д.)

            // Цикл працює, поки ми не вийдемо за межі екрана
            for (int i = step; i * drawScale < cx || i * drawScale < cy; i += step)
            {
                int px = i * drawScale; // Переведення математичних одиниць у пікселі екрана

                // Малювання поділок на осі X (якщо поміщаються на екран)
                if (px < cx)
                {
                    // Права частина (додатні числа)
                    g.DrawLine(axisPen, cx + px, cy - 3, cx + px, cy + 3); // Зарубка
                    g.DrawString(i.ToString(), font, brush, cx + px - 5, cy + 5); // Текст

                    // Ліва частина (від'ємні числа)
                    g.DrawLine(axisPen, cx - px, cy - 3, cx - px, cy + 3);
                    g.DrawString((-i).ToString(), font, brush, cx - px - 10, cy + 5);
                }

                // Малювання поділок на осі Y (якщо поміщаються на екран)
                if (px < cy)
                {
                    // Верхня частина (додатні числа)
                    g.DrawLine(axisPen, cx - 3, cy - px, cx + 3, cy - px);
                    g.DrawString(i.ToString(), font, brush, cx + 5, cy - px - 5);

                    // Нижня частина (від'ємні числа)
                    g.DrawLine(axisPen, cx - 3, cy + px, cx + 3, cy + px);
                    g.DrawString((-i).ToString(), font, brush, cx + 5, cy + px - 5);
                }
            }

            // --- 3. Розрахунок та малювання кривої (Астроїди) ---
            Pen curvePen = new Pen(Color.Blue, 2); // Синій олівець товщиною 2 пікселі для кривої
            PointF? prevPoint = null;              // Змінна для зберігання попередньої точки (щоб з'єднувати їх лініями)

            // Цикл по параметру t від 0 до 2π (одне повне коло) з кроком 0.01 (для плавності)
            for (double t = 0; t <= 2 * Math.PI; t += 0.01)
            {
                // Математичні параметричні рівняння астроїди
                double x = R * Math.Pow(Math.Cos(t), 3); // x = R * cos^3(t)
                double y = R * Math.Pow(Math.Sin(t), 3); // y = R * sin^3(t)

                // Перехід від математичних координат до пікселів на екрані:
                // Додаємо cx/cy, щоб змістити центр з лівого верхнього кута (0,0) у центр вікна.
                // Вісь Y віднімається (cy - ...), тому що в комп'ютерній графіці Y росте вниз, а в математиці - вгору.
                float screenX = cx + (float)(x * drawScale);
                float screenY = cy - (float)(y * drawScale);

                PointF currentPoint = new PointF(screenX, screenY); // Створюємо поточну точку

                // Якщо це не перша ітерація циклу і в нас вже є попередня точка
                if (prevPoint.HasValue)
                {
                    // Малюємо лінію між попередньою та поточною точкою
                    g.DrawLine(curvePen, prevPoint.Value, currentPoint);
                }

                // Зберігаємо поточну точку як "попередню" для наступного кроку циклу
                prevPoint = currentPoint;
            }
        }
    }

    // Головний клас програми, який містить точку входу (Main)
    static class Program
    {
        // Атрибут, необхідний для роботи з COM-компонентами (наприклад, діалоговими вікнами типу SaveFileDialog)
        [STAThread]
        static void Main()
        {
            // Увімкнення візуальних стилів ОС Windows (щоб кнопки виглядали сучасно)
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Запуск головного вікна програми
            Application.Run(new MainForm());
        }
    }
}