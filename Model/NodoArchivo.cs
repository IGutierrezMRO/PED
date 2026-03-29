using System;
using System.Collections.Generic;
using PED.Enums;

namespace PED.Model;

public class NodoArchivo
{
    public string Nombre { get; set; }
    public TipoNodo Tipo { get; set; }

    public List<NodoArchivo> Hijos { get; } = new();

    public NodoArchivo? Padre { get; private set; }

    public bool EsCarpeta => Tipo == TipoNodo.Carpeta;

    public NodoArchivo(string nombre, TipoNodo tipo)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre no puede ser vacío.", nameof(nombre));

        Nombre = nombre;
        Tipo = tipo;
    }

    public void AgregarHijo(NodoArchivo hijo)
    {
        if (hijo is null) throw new ArgumentNullException(nameof(hijo));
        if (!EsCarpeta)
            throw new InvalidOperationException("Un archivo no puede tener hijos.");

        foreach (var h in Hijos)
            if (string.Equals(h.Nombre, hijo.Nombre, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Ya existe un hijo con ese nombre en esta carpeta.");

        hijo.Padre = this;
        Hijos.Add(hijo);
    }
}