using System;
using AriesContador.Core.Models.Utils;
using System.Collections;
using System.Collections.Generic;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.Accounts;
using DocumentFormat.OpenXml.Spreadsheet;

namespace AriesContador.Core.Models.Companies
{
    public class Company : BaseModel
    {
        public string CopyFrom { get; set; }
        public string Code { get; set; }

        public string CompanyName { get; set; }

        /// <summary>Alias legacy (WinForms / CapaEntidad) de CompanyName.</summary>
        public string Name
        {
            get => CompanyName;
            set => CompanyName = value;
        }

        public CompanyType CompanyType { get; set;  }

        public IdType IdType { get; set; }

        public string NumberId { get; set; }

        /// <summary>Alias legacy de NumberId.</summary>
        public string IdNumber
        {
            get => NumberId;
            set => NumberId = value;
        }

        public string Op1 { get; set; }

        public string Op2 { get; set; }

        public string Address { get; set; }

        public string Mail { get; set; }

        public string PhoneNumber1 { get; set; }

        public string PhoneNumber2 { get; set; }

        public string Notes { get; set; }

        /// <summary>Alias legacy de Notes.</summary>
        public string Memo
        {
            get => Notes;
            set => Notes = value;
        }

        public string WebSite { get; set; }

        /// <summary>Alias legacy de WebSite.</summary>
        public string Web
        {
            get => WebSite;
            set => WebSite = value;
        }

        public CurrencyTypeCompany MoneyType { get; set; }

        /// <summary>Alias legacy de MoneyType.</summary>
        public CurrencyTypeCompany CurrencyType
        {
            get => MoneyType;
            set => MoneyType = value;
        }

        public IEnumerable<PostingPeriod> PostingPeriods { get; set; } = new List<PostingPeriod>();

        public IEnumerable<Account> Account { get; set; } = new List<Account>(); 

        public override string ToString()
        {
            return $"{ CompanyName.ToUpper()}-{Code}";
        }

        public Company() { }
        protected Company(IdType tipoID, string numeroId, string nombre, CurrencyTypeCompany TipoMoneda, string direccion,
                         string[] telefono, string web, string correo, string observaciones, string codigo = "", Boolean activo = true)
        {
            this.Code = codigo;
            this.IdType = tipoID;
            this.NumberId = numeroId;
            this.CompanyName = nombre;
            this.MoneyType = TipoMoneda;
            this.Address = direccion;
            this.PhoneNumber1 = telefono[0];
            this.PhoneNumber2 = telefono[1];
            this.WebSite = web;
            this.Mail = correo;
            this.Notes = observaciones;
            this.Active = activo;
        }
    }

    public class PersonaFisica : Company
    {
        private String apellidoPaterno;
        private String apellidoMaterno;


        public PersonaFisica()
        {
        }

        public PersonaFisica(IdType tipoID, string numeroId, string nombre, CurrencyTypeCompany TipoMoneda, string direccion,
                                string[] telefono, string web, string correo, string observaciones, String apellidoPaterno, String apellidoMaterno, string codigo = "", Boolean activo = true) :
                                base(tipoID, numeroId, nombre, TipoMoneda, direccion, telefono, web, correo, observaciones, codigo, activo)
        {
            this.apellidoPaterno = apellidoPaterno;
            this.apellidoMaterno = apellidoMaterno;
        }
        //public PersonaFisica(String apelledoPaterno, String apellidoMaterno)
        //{
        //    this.apellidoPaterno = apelledoPaterno;
        //    this.MyApellidoMaterno = apellidoMaterno;
        //}

        public String MyApellidoPaterno
        {
            get { return apellidoPaterno; }
            set { apellidoPaterno = value; }
        }

        public String MyApellidoMaterno
        {
            get { return apellidoMaterno; }
            set { apellidoMaterno = value; }
        }


    }

    public class PersonaJuridica : Company
    {
        private String representanteLegal;
        private String IDRepresentante;
        public PersonaJuridica()
        {
        }

        public PersonaJuridica(IdType tipoID, string numeroId, string nombre, CurrencyTypeCompany TipoMoneda, string direccion,
                                  string[] telefono, string web, string correo, string observaciones,
                                  String representanteLegal, String IDRepresentante, string codigo = "", Boolean activo = true) :
                                  base(tipoID, numeroId, nombre, TipoMoneda, direccion, telefono, web, correo, observaciones, codigo, activo)
        {
            this.representanteLegal = representanteLegal;
            this.IDRepresentante = IDRepresentante;
        }

        public String MyRepresentanteLegal
        {
            get { return representanteLegal; }
            set { representanteLegal = value; }
        }

        public String MyIDRepresentanteLegal
        {
            get { return IDRepresentante; }
            set { IDRepresentante = value; }
        }

    }
}
