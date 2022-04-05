﻿using System.Drawing;
using System.Drawing.Drawing2D;
using Zappy.ZappyTaskEditor.EditorPage;

namespace Zappy.ZappyTaskEditor.Nodes
{
    public sealed class NodeStyle
    {
        public Font Font { get; set; }

        public Brush FontBrush { get; set; }

        public Pen BorderPen { get; set; }

        public Brush PortBackBrush { get; set; }

        public Brush BackBrush { get; set; }

        public Pen LinePenWithArrow { get; set; }


        public NodeStyle()
        {
            this.Font = new Font("Segoe UI", 10, GraphicsUnit.Pixel);

            this.FontBrush = new SolidBrush(Color.FromArgb(50, 50, 50));

            this.BorderPen = new Pen(Color.FromArgb(100, 100, 100), 1.5f);
            this.BackBrush = new SolidBrush(PageRenderOptions.BackColor);
            this.PortBackBrush = new SolidBrush(Color.FromArgb(150, 50, 150, 250));
            this.LinePenWithArrow = new Pen(Color.FromArgb(100, 100, 100), 1.5f);

            using (GraphicsPath endCap = new GraphicsPath())
            {
                var width = PageRenderOptions.GridSize * 3 / 4 / 2 / this.LinePenWithArrow.Width;

                endCap.AddLine(-width, -width - this.LinePenWithArrow.Width / 2, 0, -this.LinePenWithArrow.Width / 2);
                endCap.AddLine(+width, -width - this.LinePenWithArrow.Width / 2, 0, -this.LinePenWithArrow.Width / 2);

                this.LinePenWithArrow.CustomEndCap = new CustomLineCap(null, endCap);
            }
        }

        public StringFormat StringFormat = new StringFormat()
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };
    }
}