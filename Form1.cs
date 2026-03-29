using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PED.Enums;
using PED.Model;

namespace PED;

public partial class Form1 : Form
{
    private readonly ArbolSistemaArchivos arbol = new();

    private readonly Dictionary<NodoArchivo, Point> pos = new();
    private const int NodeW = 180;
    private const int NodeH = 30;
    private const int XGap  = 220;
    private const int YGap  = 55;
    private const int Margin = 40;

    public Form1()
    {
        InitializeComponent();
        DoubleBuffered = true;

        AutoScroll = true;

        ConstruirArbolEjemplo();

        Paint += Form1_Paint;
        Resize += (_, __) => Invalidate();
    }

    private void ConstruirArbolEjemplo()
    {
        arbol.AgregarNodo("/root", "documentos", TipoNodo.Carpeta);
        arbol.AgregarNodo("/root/documentos", "HV.pdf", TipoNodo.Archivo);
        arbol.AgregarNodo("/root/documentos", "proyectoAplicado.pdf", TipoNodo.Archivo);
        arbol.AgregarNodo("/root/documentos", "inscripcion.docx", TipoNodo.Archivo);
        

        arbol.AgregarNodo("/root", "fotos", TipoNodo.Carpeta);
        arbol.AgregarNodo("/root/fotos", "convenciones", TipoNodo.Carpeta);
        arbol.AgregarNodo("/root/fotos/convenciones", "hackathon.jpg", TipoNodo.Archivo);
        arbol.AgregarNodo("/root/fotos/convenciones", "midudev.jpg", TipoNodo.Archivo);
        arbol.AgregarNodo("/root/fotos/convenciones", "proyecto.png", TipoNodo.Archivo);
        arbol.AgregarNodo("/root/fotos", "vacaciones", TipoNodo.Carpeta);
        arbol.AgregarNodo("/root/fotos/vacaciones", "Colombia", TipoNodo.Carpeta);
        arbol.AgregarNodo("/root/fotos/vacaciones/Colombia", "monserrate.png", TipoNodo.Archivo);
        arbol.AgregarNodo("/root/fotos/vacaciones/Colombia", "chorroQuevedo.png", TipoNodo.Archivo);
        arbol.AgregarNodo("/root/fotos", "perfil.jpg", TipoNodo.Archivo);

        arbol.AgregarNodo("/root", "sistema", TipoNodo.Carpeta);
        arbol.AgregarNodo("/root/sistema", "application.properties", TipoNodo.Archivo);
        arbol.AgregarNodo("/root", "configuraciones", TipoNodo.Carpeta);
        arbol.AgregarNodo("/root/configuraciones", "appsettings.json", TipoNodo.Archivo);
    }

    private void Form1_Paint(object? sender, PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        e.Graphics.TranslateTransform(AutoScrollPosition.X, AutoScrollPosition.Y);

        pos.Clear();

        int leafIndex = 0;
        CalcularPosiciones(arbol.Root, depth: 0, ref leafIndex);


        AjustarScrollArea();

        DibujarLineas(e.Graphics, arbol.Root);

        DibujarNodos(e.Graphics, arbol.Root);
    }

    private void CalcularPosiciones(NodoArchivo nodo, int depth, ref int leafIndex)
    {
        int x = Margin + depth * XGap;

        if (!nodo.EsCarpeta || nodo.Hijos.Count == 0)
        {
            int y = Margin + leafIndex * YGap;
            pos[nodo] = new Point(x, y);
            leafIndex++;
            return;
        }

        foreach (var hijo in nodo.Hijos)
            CalcularPosiciones(hijo, depth + 1, ref leafIndex);

        int minY = nodo.Hijos.Min(h => pos[h].Y);
        int maxY = nodo.Hijos.Max(h => pos[h].Y);
        int centerY = (minY + maxY) / 2;

        pos[nodo] = new Point(x, centerY);
    }

    private void AjustarScrollArea()
    {
        if (pos.Count == 0) return;

        int maxX = pos.Max(kv => kv.Value.X) + NodeW + Margin;
        int maxY = pos.Max(kv => kv.Value.Y) + NodeH + Margin;

        AutoScrollMinSize = new Size(maxX, maxY);
    }

    private void DibujarLineas(Graphics g, NodoArchivo nodo)
    {
        if (!pos.TryGetValue(nodo, out var pPadre)) return;

        if (nodo.EsCarpeta)
        {
            foreach (var hijo in nodo.Hijos)
            {
                if (pos.TryGetValue(hijo, out var pHijo))
                {
                    var from = new Point(pPadre.X + NodeW, pPadre.Y + NodeH / 2);
                    var to   = new Point(pHijo.X,        pHijo.Y + NodeH / 2);

                    int midX = (from.X + to.X) / 2;
                    g.DrawLine(Pens.Black, from, new Point(midX, from.Y));
                    g.DrawLine(Pens.Black, new Point(midX, from.Y), new Point(midX, to.Y));
                    g.DrawLine(Pens.Black, new Point(midX, to.Y), to);

                    DibujarLineas(g, hijo);
                }
            }
        }
    }

    private void DibujarNodos(Graphics g, NodoArchivo nodo)
    {
        if (!pos.TryGetValue(nodo, out var p)) return;

        var rect = new Rectangle(p.X, p.Y, NodeW, NodeH);

        Brush brush = nodo.Tipo == TipoNodo.Carpeta ? Brushes.LightGray : Brushes.White;

        g.FillRectangle(brush, rect);
        g.DrawRectangle(Pens.Black, rect);

        string label = nodo.Tipo == TipoNodo.Carpeta ? $"{nodo.Nombre}/" : nodo.Nombre;
        g.DrawString(label, Font, Brushes.Black, p.X + 6, p.Y + 7);

        if (nodo.EsCarpeta)
        {
            foreach (var hijo in nodo.Hijos)
                DibujarNodos(g, hijo);
        }
    }
}