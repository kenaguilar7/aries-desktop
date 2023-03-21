//using AriesContador.Core.Models.Utils;
using CapaEntidad.Entidades.FechaTransacciones;
using CapaEntidad.Enumeradores;
using System;
using System.Collections.Generic;

namespace CapaEntidad.Entidades.Compañias
{
    //public class Company : BaseModel
    //{
    //    public CompanyType CompanyType { get; set; }

    //    public string Code { get; set; }
    //    public TipoID IdType { get; set; }
    //    public string NumeroCedula { get; set; }
    //    public string Name { get; set; }
    //    public string Address { get; set; }
    //    public string Web { get; set; }
    //    public string Mail { get; set; }
    //    public string PhoneNumber1 { get; set; }

    //    public string PhoneNumber2 { get; set; }
    //    public string[] Telefono  { get; set; }
    //    public string Memo { get; set; }
    //    //public bool Active { get; set; }
    //    public CurrencyTypeCompany CurrencyType { get; set; }
    //    public List<FechaTransaccion> PostingPeriods { get; set; }
    //    public Company() { }

    //    protected Company(TipoID tipoID, string numeroId, string nombre, CurrencyTypeCompany TipoMoneda, string direccion,
    //                     string[] telefono, string web, string correo, string observaciones, string codigo = "", Boolean activo = true)
    //    {
    //        this.Code = codigo;
    //        this.IdType = tipoID;
    //        this.NumeroCedula = numeroId;
    //        this.Name = nombre;
    //        this.CurrencyType = TipoMoneda;
    //        this.Address = direccion;
    //        this.Telefono = telefono;
    //        this.Web = web;
    //        this.Mail = correo;
    //        this.Memo = observaciones;
    //        this.Active = activo;
    //    }


    //    public override string ToString()
    //        => $"{ Name.ToUpper()}-{Code}"; 


    //}

    //public enum CompanyType
    //{
    //    Persona_Jurídica = 1,
    //    Persona_Física = 2
    //}

    //public enum IdType
    //{
    //    CEDULA_JURIDICA = 1,
    //    CEDULA_NACIONAL = 2,
    //    DIMEX = 3,
    //    NITE = 4,
    //}
    //public enum CurrencyTypeCompany
    //{
    //    Dolares_y_Colones = 1,
    //    Solo_Colones = 2,
    //    Solo_Dolares = 3
    //}

    //public class PersonaFisica : Company
    //{
    //    private String apellidoPaterno;
    //    private String apellidoMaterno;


    //    public PersonaFisica()
    //    {
    //    }

    //    public PersonaFisica(TipoID tipoID, string numeroId, string nombre, CurrencyTypeCompany TipoMoneda, string direccion,
    //                            string[] telefono, string web, string correo, string observaciones, String apellidoPaterno, String apellidoMaterno, string codigo = "", Boolean activo = true) :
    //                            base(tipoID, numeroId, nombre, TipoMoneda, direccion, telefono, web, correo, observaciones, codigo, activo)
    //    {
    //        this.apellidoPaterno = apellidoPaterno;
    //        this.apellidoMaterno = apellidoMaterno;
    //    }
    //    public PersonaFisica(String apelledoPaterno, String apellidoMaterno)
    //    {
    //        this.apellidoPaterno = apelledoPaterno;
    //        this.MyApellidoMaterno = apellidoMaterno;
    //    }

    //    public String MyApellidoPaterno
    //    {
    //        get { return apellidoPaterno; }
    //        set { apellidoPaterno = value; }
    //    }

    //    public String MyApellidoMaterno
    //    {
    //        get { return apellidoMaterno; }
    //        set { apellidoMaterno = value; }
    //    }


    //}

    //public class PersonaJuridica : Company
    //{
    //    private String representanteLegal;
    //    private String IDRepresentante;
    //    public PersonaJuridica()
    //    {
    //    }

    //    public PersonaJuridica(TipoID tipoID, string numeroId, string nombre, CurrencyTypeCompany TipoMoneda, string direccion,
    //                              string[] telefono, string web, string correo, string observaciones,
    //                              String representanteLegal, String IDRepresentante, string codigo = "", Boolean activo = true) :
    //                              base(tipoID, numeroId, nombre, TipoMoneda, direccion, telefono, web, correo, observaciones, codigo, activo)
    //    {
    //        this.representanteLegal = representanteLegal;
    //        this.IDRepresentante = IDRepresentante;
    //    }

    //    public String MyRepresentanteLegal
    //    {
    //        get { return representanteLegal; }
    //        set { representanteLegal = value; }
    //    }

    //    public String MyIDRepresentanteLegal
    //    {
    //        get { return IDRepresentante; }
    //        set { IDRepresentante = value; }
    //    }

    //}
}
