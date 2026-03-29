using System;
using System.Collections.Generic;
using PED.Enums;

namespace PED.Model;

public class ArbolSistemaArchivos
{
    public NodoArchivo Root { get; }

    public ArbolSistemaArchivos()
    {
        Root = new NodoArchivo("root", TipoNodo.Carpeta);
    }

    public NodoArchivo? BuscarPorRuta(string ruta)
    {
        if (string.IsNullOrWhiteSpace(ruta) || !ruta.StartsWith("/"))
            return null;

        var partes = ruta.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (partes.Length == 0 || !string.Equals(partes[0], "root", StringComparison.OrdinalIgnoreCase))
            return null;

        NodoArchivo actual = Root;

        for (int i = 1; i < partes.Length; i++)
        {
            if (!actual.EsCarpeta) return null;

            NodoArchivo? siguiente = null;
            foreach (var h in actual.Hijos)
            {
                if (string.Equals(h.Nombre, partes[i], StringComparison.OrdinalIgnoreCase))
                {
                    siguiente = h;
                    break;
                }
            }

            if (siguiente == null) return null;
            actual = siguiente;
        }

        return actual;
    }

    public NodoArchivo AgregarNodo(string rutaPadre, string nombre, TipoNodo tipo)
    {
        var padre = BuscarPorRuta(rutaPadre)
                    ?? throw new ArgumentException("Ruta padre no encontrada.");

        if (!padre.EsCarpeta)
            throw new ArgumentException("La ruta padre no es una carpeta.");

        var nuevo = new NodoArchivo(nombre, tipo);
        padre.AgregarHijo(nuevo);
        return nuevo;
    }

    public List<string> Preorden()
    {
        var outList = new List<string>();
        Preorden(Root, outList);
        return outList;
    }

    private void Preorden(NodoArchivo n, List<string> outList)
    {
        outList.Add(Formato(n));

        if (n.EsCarpeta)
            foreach (var h in n.Hijos)
                Preorden(h, outList);
    }

    public List<string> Postorden()
    {
        var outList = new List<string>();
        Postorden(Root, outList);
        return outList;
    }

    private void Postorden(NodoArchivo n, List<string> outList)
    {
        if (n.EsCarpeta)
            foreach (var h in n.Hijos)
                Postorden(h, outList);

        outList.Add(Formato(n));
    }

    public string RutaAbsoluta(NodoArchivo nodo)
    {
        var stack = new Stack<string>();
        NodoArchivo? actual = nodo;

        while (actual != null)
        {
            stack.Push(actual.Nombre);
            actual = actual.Padre;
        }

        return "/" + string.Join("/", stack);
    }

    private static string Formato(NodoArchivo n)
        => n.Tipo == TipoNodo.Carpeta ? $"{n.Nombre}/" : n.Nombre;
}