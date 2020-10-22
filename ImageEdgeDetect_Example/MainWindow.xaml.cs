using ImageEdgeDetect_Example.Inspect;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Brushes = System.Windows.Media.Brushes;

namespace ImageEdgeDetect_Example
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private CvsEdgeDetect m_EdgeDetect;
        private double m_Scale = 1;
        public MainWindow()
        {
            InitializeComponent();
            //검사 인스턴스 초기화
            m_EdgeDetect = new CvsEdgeDetect();
        }

        //이미지 추가
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog
            {
                Filter = "Bitmap Mono Image Files. (*.bmp)|*.bmp"
            };
            if ((bool)d.ShowDialog())
            {
                //그래픽 초기화
                this.ResetGraphics();

                //이미지 불러오기
                var img = new Bitmap(d.FileName);
                Bitmap bmp = null;
                //이미지 모노로 변경
                if (img.PixelFormat != System.Drawing.Imaging.PixelFormat.Format8bppIndexed) bmp = AForge.Imaging.Filters.Grayscale.CommonAlgorithms.BT709.Apply(img);
                else bmp = img;
                //이미지 화면에 디스플레이
                img_Image.Source = new BitmapImage(new Uri(d.FileName));
                //검사 이미지 추가
                m_EdgeDetect.DetectImage = bmp;
            }
        }

        //크기 변경에 따른 에지 변경
        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // 에지 찾을 이미지 없으면 반환
            if (m_EdgeDetect.DetectImage == null) return;
            // 스케일 값 구하기
            m_Scale = img_Image.ActualWidth / m_EdgeDetect.DetectImage.Width;
            //에지 있으면 원래 길이 * 스케일 로 실제 화면에 뿌리기
            if (canvas_Canvas.Children.OfType<Line>().Count() > 0)
            {
                var line = canvas_Canvas.Children.OfType<Line>().First();
                line.Y1 = m_EdgeDetect.Edge.Y * m_Scale;
                line.Y2 = m_EdgeDetect.Edge.Y * m_Scale;
            }
        }

        //에지 찾기
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //그래픽 초기화
            this.ResetGraphics();
            
            //설정값 받기
            m_EdgeDetect.ContrastThreshold = uint.Parse(threshold_TextBox.Text);
            m_EdgeDetect.HalfPixelCount = uint.Parse(halfPiixel_TextBox.Text);

            //검사
            m_EdgeDetect.Detect();
            
            //에지 그리기
            Line line = new Line
            {
                X1 = 0,
                Y1 = m_EdgeDetect.Edge.Y * m_Scale,
                X2 = img_Image.ActualWidth,
                Y2 = m_EdgeDetect.Edge.Y * m_Scale,
                Stroke = Brushes.Green,
                StrokeThickness = 5,
            };

            //캔버스에 추가
            canvas_Canvas.Children.Add(line);
        }

        private void ResetGraphics()
        {
            //캔버스에 그려진 모든 에지 찾기
            var list = canvas_Canvas.Children.OfType<Line>();
            //모든 에지 제거
            for(int i = 0;i<list.Count(); i++) canvas_Canvas.Children.Remove(list.ElementAt(i));
        }
    }
}
