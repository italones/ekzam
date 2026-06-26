using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace DrawImageApp
{
    public partial class Form1 : Form
    {
        private Bitmap canvas; // Основное изображение для рисования
        private Image loadedImage; // Загруженное изображение
        private bool isDrawing = false;
        private Point startPoint;
        private Point endPoint;
        private Point imagePosition = new Point(50, 50);
        private float imageScale = 1.0f;
        private float imageRotation = 0f;
        private bool showGrid = true;

        public Form1()
        {
            InitializeComponent();
            InitializeCanvas();
            LoadSampleImage();
        }

        private void InitializeCanvas()
        {
            // Создаем холст размером с pictureBox
            canvas = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.Clear(Color.White);
                DrawGrid(g);
            }
            pictureBox1.Image = canvas;
        }

        private void LoadSampleImage()
        {
            try
            {
                // Создаем тестовое изображение
                loadedImage = CreateTestImage();
                RefreshCanvas();
            }
            catch
            {
                // Если не удалось создать, используем встроенную картинку
                loadedImage = new Bitmap(100, 100);
                using (Graphics g = Graphics.FromImage(loadedImage))
                {
                    g.FillRectangle(Brushes.Red, 0, 0, 100, 100);
                }
            }
        }

        private Image CreateTestImage()
        {
            // Создаем красивое тестовое изображение
            Bitmap bmp = new Bitmap(200, 150);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // Фон
                LinearGradientBrush gradient = new LinearGradientBrush(
                    new Rectangle(0, 0, 200, 150),
                    Color.FromArgb(52, 152, 219),
                    Color.FromArgb(155, 89, 182),
                    LinearGradientMode.ForwardDiagonal);
                g.FillRectangle(gradient, 0, 0, 200, 150);

                // Рисуем текст
                g.DrawString("Sample\nImage",
                    new Font("Arial", 20, FontStyle.Bold),
                    Brushes.White,
                    new RectangleF(20, 30, 160, 100));

                // Рисуем рамку
                g.DrawRectangle(Pens.White, 5, 5, 190, 140);

                // Рисуем узор
                for (int i = 0; i < 200; i += 20)
                {
                    g.DrawLine(new Pen(Color.FromArgb(100, 255, 255, 255), 2),
                        i, 0, i + 20, 150);
                }
            }
            return bmp;
        }

        // =============== МЕТОДЫ РИСОВАНИЯ ===============

        // Простое рисование изображения
        private void DrawImageSimple(Graphics g, Image image, Point position)
        {
            g.DrawImage(image, position.X, position.Y);
        }

        // Рисование с масштабированием
        private void DrawImageScaled(Graphics g, Image image, Point position, float scale)
        {
            int width = (int)(image.Width * scale);
            int height = (int)(image.Height * scale);
            g.DrawImage(image, position.X, position.Y, width, height);
        }

        // Рисование с поворотом
        private void DrawImageRotated(Graphics g, Image image, Point position, float angle)
        {
            g.TranslateTransform(position.X + image.Width / 2, position.Y + image.Height / 2);
            g.RotateTransform(angle);
            g.TranslateTransform(-image.Width / 2, -image.Height / 2);
            g.DrawImage(image, 0, 0);
            g.ResetTransform();
        }

        // Рисование с прозрачностью
        private void DrawImageTransparent(Graphics g, Image image, Point position, float opacity)
        {
            ColorMatrix colorMatrix = new ColorMatrix();
            colorMatrix.Matrix33 = opacity; // Устанавливаем прозрачность

            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(colorMatrix);

            g.DrawImage(image,
                new Rectangle(position.X, position.Y, image.Width, image.Height),
                0, 0, image.Width, image.Height,
                GraphicsUnit.Pixel,
                attributes);
        }

        // Рисование с эффектами
        private void DrawImageWithEffects(Graphics g, Image image, Point position)
        {
            // Сохраняем состояние
            GraphicsState state = g.Save();

            // Создаем эффект тени
            g.TranslateTransform(position.X + 5, position.Y + 5);
            g.DrawImage(image, 0, 0);
            g.ResetTransform();

            // Рисуем основное изображение
            g.DrawImage(image, position.X, position.Y);

            // Восстанавливаем состояние
            g.Restore(state);
        }

        // Рисование сетки
        private void DrawGrid(Graphics g)
        {
            if (!showGrid) return;

            Pen gridPen = new Pen(Color.FromArgb(50, 200, 200, 200), 1);
            int gridSize = 50;

            for (int x = 0; x < pictureBox1.Width; x += gridSize)
            {
                g.DrawLine(gridPen, x, 0, x, pictureBox1.Height);
            }

            for (int y = 0; y < pictureBox1.Height; y += gridSize)
            {
                g.DrawLine(gridPen, 0, y, pictureBox1.Width, y);
            }

            // Рисуем центральные линии
            Pen centerPen = new Pen(Color.FromArgb(100, 255, 0, 0), 2);
            g.DrawLine(centerPen, pictureBox1.Width / 2, 0,
                pictureBox1.Width / 2, pictureBox1.Height);
            g.DrawLine(centerPen, 0, pictureBox1.Height / 2,
                pictureBox1.Width, pictureBox1.Height / 2);
        }

        // Обновление холста
        private void RefreshCanvas()
        {
            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.Clear(Color.White);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                // Рисуем сетку
                DrawGrid(g);

                if (loadedImage != null)
                {
                    // Показываем все способы рисования
                    int yPos = 20;

                    // 1. Обычное рисование
                    g.DrawString("1. Обычное", new Font("Arial", 10), Brushes.Black, 10, yPos - 15);
                    DrawImageSimple(g, loadedImage, new Point(10, yPos));

                    // 2. Масштабированное
                    g.DrawString("2. Масштаб 1.5x", new Font("Arial", 10), Brushes.Black, 230, yPos - 15);
                    DrawImageScaled(g, loadedImage, new Point(230, yPos), 1.5f);

                    // 3. С поворотом
                    g.DrawString("3. Поворот 45°", new Font("Arial", 10), Brushes.Black, 450, yPos - 15);
                    DrawImageRotated(g, loadedImage, new Point(450, yPos), 45);

                    yPos += loadedImage.Height + 30;

                    // 4. С прозрачностью
                    g.DrawString("4. Прозрачность 50%", new Font("Arial", 10), Brushes.Black, 10, yPos - 15);
                    DrawImageTransparent(g, loadedImage, new Point(10, yPos), 0.5f);

                    // 5. С эффектами
                    g.DrawString("5. С эффектами", new Font("Arial", 10), Brushes.Black, 230, yPos - 15);
                    DrawImageWithEffects(g, loadedImage, new Point(230, yPos));

                    // 6. С границами
                    g.DrawString("6. С рамкой", new Font("Arial", 10), Brushes.Black, 450, yPos - 15);
                    g.DrawImage(loadedImage, new Point(450, yPos));
                    g.DrawRectangle(Pens.Red, new Rectangle(450, yPos,
                        loadedImage.Width, loadedImage.Height));

                    // Дополнительная информация
                    string info = $"Размер: {loadedImage.Width}x{loadedImage.Height}px | " +
                                 $"Масштаб: {imageScale:F1}x | " +
                                 $"Поворот: {imageRotation}°";
                    g.DrawString(info, new Font("Arial", 9), Brushes.Gray, 10,
                        pictureBox1.Height - 30);
                }
            }
            pictureBox1.Invalidate();
        }

        // =============== ОБРАБОТЧИКИ СОБЫТИЙ ===============

        // Загрузка изображения
        private void btnLoadImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                openFileDialog.Title = "Выберите изображение";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        loadedImage = Image.FromFile(openFileDialog.FileName);
                        imagePosition = new Point(50, 50);
                        imageScale = 1.0f;
                        imageRotation = 0f;
                        RefreshCanvas();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка загрузки: {ex.Message}");
                    }
                }
            }
        }

        // Сохранение изображения
        private void btnSaveImage_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg|BMP Image|*.bmp";
                saveFileDialog.Title = "Сохранить изображение";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        canvas.Save(saveFileDialog.FileName);
                        MessageBox.Show("Изображение сохранено!", "Успех");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка сохранения: {ex.Message}");
                    }
                }
            }
        }

        // Изменение масштаба
        private void trackBarScale_Scroll(object sender, EventArgs e)
        {
            imageScale = trackBarScale.Value / 10f;
            lblScale.Text = $"Масштаб: {imageScale:F1}x";
            RefreshCanvas();
        }

        // Изменение поворота
        private void trackBarRotation_Scroll(object sender, EventArgs e)
        {
            imageRotation = trackBarRotation.Value;
            lblRotation.Text = $"Поворот: {imageRotation}°";
            RefreshCanvas();
        }

        // Отображение сетки
        private void chkShowGrid_CheckedChanged(object sender, EventArgs e)
        {
            showGrid = chkShowGrid.Checked;
            RefreshCanvas();
        }

        // Применение эффектов
        private void btnApplyEffects_Click(object sender, EventArgs e)
        {
            RefreshCanvas();
        }

        // Очистка холста
        private void btnClearCanvas_Click(object sender, EventArgs e)
        {
            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.Clear(Color.White);
                DrawGrid(g);
            }
            pictureBox1.Invalidate();
        }

        // Сброс настроек
        private void btnReset_Click(object sender, EventArgs e)
        {
            trackBarScale.Value = 10;
            trackBarRotation.Value = 0;
            imageScale = 1.0f;
            imageRotation = 0f;
            imagePosition = new Point(50, 50);
            showGrid = true;
            chkShowGrid.Checked = true;
            RefreshCanvas();
        }

        // Изменение размера окна
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (pictureBox1.Width > 0 && pictureBox1.Height > 0)
            {
                // Пересоздаем холст при изменении размера
                canvas = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                RefreshCanvas();
            }
        }

        // Рисование с помощью мыши
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            isDrawing = true;
            startPoint = e.Location;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                endPoint = e.Location;
                // Показываем координаты
                lblCoordinates.Text = $"Позиция: ({endPoint.X}, {endPoint.Y})";
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDrawing && loadedImage != null)
            {
                // Рисуем изображение в месте клика
                using (Graphics g = Graphics.FromImage(canvas))
                {
                    g.DrawImage(loadedImage, e.X - 50, e.Y - 50, 100, 100);
                }
                pictureBox1.Invalidate();
                isDrawing = false;
            }
        }

        // =============== РАСШИРЕННЫЕ МЕТОДЫ РИСОВАНИЯ ===============

        // Рисование с размытием
        private void DrawImageBlur(Graphics g, Image image, Point position)
        {
            // Создаем эффект размытия с помощью матрицы
            float[][] matrixItems = new float[][]
            {
                new float[] {1/9f, 1/9f, 1/9f, 0, 0},
                new float[] {1/9f, 1/9f, 1/9f, 0, 0},
                new float[] {1/9f, 1/9f, 1/9f, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {0, 0, 0, 0, 1}
            };

            ColorMatrix colorMatrix = new ColorMatrix(matrixItems);
            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(colorMatrix);

            g.DrawImage(image,
                new Rectangle(position.X, position.Y, image.Width, image.Height),
                0, 0, image.Width, image.Height,
                GraphicsUnit.Pixel,
                attributes);
        }

        // Рисование с эффектом сепии
        private void DrawImageSepia(Graphics g, Image image, Point position)
        {
            float[][] matrixItems = new float[][]
            {
                new float[] {0.393f, 0.349f, 0.272f, 0, 0},
                new float[] {0.769f, 0.686f, 0.534f, 0, 0},
                new float[] {0.189f, 0.168f, 0.131f, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {0, 0, 0, 0, 1}
            };

            ColorMatrix colorMatrix = new ColorMatrix(matrixItems);
            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(colorMatrix);

            g.DrawImage(image,
                new Rectangle(position.X, position.Y, image.Width, image.Height),
                0, 0, image.Width, image.Height,
                GraphicsUnit.Pixel,
                attributes);
        }

        // Рисование с закругленными углами
        private void DrawImageRoundedCorners(Graphics g, Image image, Point position, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            Rectangle rect = new Rectangle(position.X, position.Y, image.Width, image.Height);

            // Создаем фигуру с закругленными углами
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.X + rect.Width - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.X + rect.Width - radius, rect.Y + rect.Height - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Y + rect.Height - radius, radius, radius, 90, 90);
            path.CloseFigure();

            // Рисуем изображение внутри фигуры
            g.SetClip(path);
            g.DrawImage(image, rect);
            g.ResetClip();
        }

        // Рисование с водяным знаком
        private void DrawImageWithWatermark(Graphics g, Image image, Point position, string watermark)
        {
            // Рисуем изображение
            g.DrawImage(image, position.X, position.Y);

            // Добавляем водяной знак
            using (Font font = new Font("Arial", 24, FontStyle.Bold))
            {
                SizeF textSize = g.MeasureString(watermark, font);
                PointF textPosition = new PointF(
                    position.X + (image.Width - textSize.Width) / 2,
                    position.Y + (image.Height - textSize.Height) / 2
                );

                // Рисуем тень
                g.DrawString(watermark, font, Brushes.Black, textPosition.X + 2, textPosition.Y + 2);
                // Рисуем текст
                g.DrawString(watermark, font, Brushes.White, textPosition);
            }
        }

        // Композитное рисование (несколько изображений)
        private void DrawCompositeImage(Graphics g)
        {
            if (loadedImage == null) return;

            // Создаем коллаж из нескольких копий изображения
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Point pos = new Point(50 + i * (loadedImage.Width + 10),
                                         50 + j * (loadedImage.Height + 10));
                    float opacity = 1.0f - (i + j) * 0.15f;
                    DrawImageTransparent(g, loadedImage, pos, Math.Max(opacity, 0.2f));
                }
            }
        }
    }
}