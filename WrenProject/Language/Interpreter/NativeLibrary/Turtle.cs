﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Language
{
    internal static class Turtle
    {
        private static Control DrawControl { get; set; }
        private static Graphics DrawGraphics { get; set; }
        private static Pen DrawPen { get; set; }

        private static DrawingPanel Forms { get; set; }

        private static float X { get; set; }
        private static float Y { get; set; }

        private static int PenWidth { get; set; }

        private static double Direction { get; set; }

        private static readonly List<Action> Functions = new();


        static Turtle()
        {
            Clear();
        }

        private static void Init()
        {
            Dispose();
            Direction = 0;
            PenWidth = 5;

            DrawControl = Forms;

            DrawPen = new Pen(Color.Black, PenWidth) {StartCap = LineCap.Round, EndCap = LineCap.Round};

            DrawGraphics = Forms.Graphics;
            Forms.Canvas.Paint += CanvasOnPaint;
            Forms.Canvas.ClientSizeChanged += ClientSizeChanged;
            X = Forms.Canvas.ClientSize.Width / 2;
            Y = Forms.Canvas.ClientSize.Height / 2;
            DrawGraphics.SmoothingMode = SmoothingMode.AntiAlias;
        }

        private static void CanvasOnPaint(object sender, PaintEventArgs e)
        {
            foreach (var func in Functions)
            {
                func();
            }
        }

        public static void Forward()
        {
            Functions.Add(() => Forward(1));
        }

        public static void Forward(double steps)
        {
            Functions.Add(() => DrawLine(steps));
        }

        public static void Right(double degrees)
        {
            Functions.Add(() => Direction += degrees);
        }

        public static void Left(double degrees)
        {
            Right(-degrees);
        }


        private static void DrawLine(double steps)
        {
            var radians = Direction * (Math.PI / 180);
            var oldX = X;
            var oldY = Y;
            X += (float) (steps * Math.Cos(radians));
            Y += (float) (steps * Math.Sin(radians));
            DrawGraphics.DrawLine(DrawPen, oldX, oldY, X, Y);
        }

        public static void Done()
        {
            Forms.ShowDialog();
        }

        public static void Clear()
        {
            Functions.Clear();
            Forms = new DrawingPanel();
            Init();
        }

        private static void ClientSizeChanged(object sender, EventArgs e)
        {
            X = Forms.Canvas.ClientSize.Width / 2;
            Y = Forms.Canvas.ClientSize.Height / 2;
            Forms.Canvas.Invalidate();
        }

        private static void Dispose()
        {
            if (DrawControl != null)
            {
                Forms.Canvas.Paint -= CanvasOnPaint;
                Forms.Canvas.ClientSizeChanged -= ClientSizeChanged;
                DrawPen.Dispose();
                DrawPen = null;
                DrawGraphics.Dispose();
                DrawGraphics = null;
                DrawControl.Invalidate();
                DrawControl = null;
            }
        }
    }
}