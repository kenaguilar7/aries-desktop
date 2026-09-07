using AriesContador.Core;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.Companies;
using AriesContador.Core.Models.Users;
using AriesContador.Core.Models.Utils;
using AriesContador.Core.Services;
using AriesContador.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AriesContador.Services
{
    public class AdministrationService : IAdministrationService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AdministrationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public WebToken Login(Login param)
        {
            if (param == null || string.IsNullOrWhiteSpace(param.UserId))
                return new WebToken();

            var user = _unitOfWork.UserRepository.FindByUserName(param.UserId);
            if (user == null || !user.Active || !PasswordMatches(user, param.Password))
                return new WebToken();

            if (!PasswordHasher.LooksHashed(user.Password))
            {
                user.Password = PasswordHasher.Hash(param.Password);
                user.UpdatedBy = user.Id;
                _unitOfWork.UserRepository.Update(user);
            }

            return new WebToken
            {
                Token = "local",
                User = ForClient(user)
            };
        }

        public async Task CreateCompany(Company compañia)
        {
            ValidateCompany(compañia);
            FlattenPersonFields(compañia);

            if (string.IsNullOrWhiteSpace(compañia.Code))
                compañia.Code = await GetCompanyConsecutive();

            compañia.Account = LoadAccountsForNewCompany(compañia);
            _unitOfWork.CompanyRepository.Add(compañia);
        }

        public void CreateUser(User usuario)
        {
            ValidateNewUser(usuario);
            if (!string.IsNullOrEmpty(usuario.Password) && !PasswordHasher.LooksHashed(usuario.Password))
                usuario.Password = PasswordHasher.Hash(usuario.Password);
            _unitOfWork.UserRepository.Add(usuario);
        }

        public Task DeleteCompany(Company company)
        {
            return _unitOfWork.CompanyRepository.Remove(company);
        }

        public Company FindByCode(string code)
        {
            var all = _unitOfWork.CompanyRepository.GetAllBlocking();
            return Hydrate(all.FirstOrDefault(c => c.Code == code));
        }

        public User FinUserById(int id)
        {
            var user = _unitOfWork.UserRepository.GetById(id);
            if (user != null)
                user.Password = null;
            return user;
        }

        public Task<IEnumerable<Company>> GetAllCompanies()
        {
            return GetAllCompanies(currentUser: null);
        }

        public async Task<IEnumerable<Company>> GetAllCompanies(User currentUser)
        {
            var companies = (await _unitOfWork.CompanyRepository.GetAll()).Select(Hydrate).ToList();

            if (currentUser != null && currentUser.UserType == UserType.Usuario)
            {
                var allowed = new HashSet<string>(await _unitOfWork.CompanyRepository.GetCodesAllowedForUser(currentUser.Id));
                companies = companies.Where(c => allowed.Contains(c.Code)).ToList();
            }

            return companies;
        }

        public IEnumerable<Company> GetAllInactiveCompanies()
        {
            return _unitOfWork.CompanyRepository.GetAllBlocking().Select(Hydrate).Where(c => !c.Active);
        }

        public IEnumerable<User> GetAllInactiveUsers()
        {
            return GetAllUsers().Where(u => !u.Active);
        }

        public IEnumerable<User> GetAllUsers()
        {
            return StripPasswords(_unitOfWork.UserRepository.GetAll());
        }

        public void InactivateUser(User usuario)
        {
            usuario.Active = false;
            PreserveStoredPassword(usuario);
            _unitOfWork.UserRepository.Update(usuario);
        }

        public void UpdateCompany(Company compania)
        {
            ValidateCompany(compania);
            FlattenPersonFields(compania);
            _unitOfWork.CompanyRepository.Update(compania);
        }

        public void UpdateUser(User usuario)
        {
            if (usuario == null || string.IsNullOrWhiteSpace(usuario.Name) || string.IsNullOrWhiteSpace(usuario.UserName))
                throw new InvalidOperationException("No se puede guardar usuarios con nombes en blanco");

            if (string.IsNullOrEmpty(usuario.Password))
                PreserveStoredPassword(usuario);
            else if (!PasswordHasher.LooksHashed(usuario.Password))
                usuario.Password = PasswordHasher.Hash(usuario.Password);
            _unitOfWork.UserRepository.Update(usuario);
        }

        public async Task<string> GetCompanyConsecutive()
        {
            var lastCompany = await _unitOfWork.CompanyRepository.LatestCode();
            return "C" + (int.Parse(lastCompany.Substring(1, 3)) + 1).ToString("000");
        }

        public bool UserNameTaken(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return false;

            var existing = _unitOfWork.UserRepository.FindByUserName(userName);
            return existing != null;
        }

        private static User ForClient(User user)
        {
            if (user == null)
                return null;

            return new User
            {
                Id = user.Id,
                UserName = user.UserName,
                UserType = user.UserType,
                IdNumber = user.IdNumber,
                Name = user.Name,
                LastName = user.LastName,
                MiddleName = user.MiddleName,
                PhoneNumber = user.PhoneNumber,
                Mail = user.Mail,
                Memo = user.Memo,
                Active = user.Active,
                CreatedBy = user.CreatedBy,
                UpdatedBy = user.UpdatedBy,
                CreatedAt = user.CreatedAt,
                UpdateAt = user.UpdateAt
            };
        }

        private void PreserveStoredPassword(User usuario)
        {
            if (usuario == null || usuario.Id <= 0)
                return;
            var stored = _unitOfWork.UserRepository.GetById(usuario.Id);
            if (stored != null)
                usuario.Password = stored.Password;
        }

        private static bool PasswordMatches(User user, string password)
        {
            if (PasswordHasher.LooksHashed(user.Password))
                return PasswordHasher.Verify(password, user.Password);
            return user.Password == password;
        }

        private static IEnumerable<User> StripPasswords(IEnumerable<User> users)
        {
            foreach (var user in users)
            {
                if (user != null)
                    user.Password = null;
                yield return user;
            }
        }

        private void ValidateNewUser(User usuario)
        {
            if (usuario == null || string.IsNullOrWhiteSpace(usuario.Name) || string.IsNullOrWhiteSpace(usuario.UserName))
                throw new InvalidOperationException("No se puede guardar usuarios con nombes en blanco");

            if (UserNameTaken(usuario.UserName))
                throw new InvalidOperationException("El nombre de usuario ya se encuentra registrado, intente con otro");
        }

        private static void ValidateCompany(Company company)
        {
            if (company == null)
                throw new InvalidOperationException("Compañía inválida");

            if (!VerificarID(company.NumberId, company.IdType, out var idMessage))
                throw new InvalidOperationException(idMessage);

            if (string.IsNullOrWhiteSpace(company.Name))
                throw new InvalidOperationException("El campo Nombre no puede estar vacio");

            if (!ValidarEmail(company.Mail))
                throw new InvalidOperationException("Formato de correo invalido");
        }

        private static bool VerificarID(string id, IdType tipoID, out string mensaje)
        {
            switch (tipoID)
            {
                case IdType.CEDULA_JURIDICA:
                case IdType.DIMEX:
                    if (id == null || id.Length != 12)
                    {
                        mensaje = "Formato de cédula incorrecto";
                        return false;
                    }
                    break;
                case IdType.CEDULA_NACIONAL:
                    if (id == null || id.Length != 11)
                    {
                        mensaje = "Formato de cédula incorrecto";
                        return false;
                    }
                    break;
                case IdType.NITE:
                    if (id == null || id.Length != 10)
                    {
                        mensaje = "Formato de cédula incorrecto";
                        return false;
                    }
                    break;
            }

            mensaje = "Formato de cédula correctos";
            return true;
        }

        private static bool ValidarEmail(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return false;
                new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private static void FlattenPersonFields(Company company)
        {
            if (company is PersonaJuridica juridica)
            {
                company.Op1 = juridica.MyIDRepresentanteLegal;
                company.Op2 = juridica.MyRepresentanteLegal;
            }
            else if (company is PersonaFisica fisica)
            {
                company.Op1 = fisica.MyApellidoPaterno;
                company.Op2 = fisica.MyApellidoMaterno;
            }
        }

        private static Company Hydrate(Company company)
        {
            if (company == null)
                return null;

            if (company is PersonaFisica || company is PersonaJuridica)
                return company;

            if (company.IdType == IdType.CEDULA_JURIDICA)
            {
                var juridica = new PersonaJuridica();
                CopyCompany(company, juridica);
                juridica.MyRepresentanteLegal = company.Op1;
                juridica.MyIDRepresentanteLegal = company.Op2;
                return juridica;
            }

            var fisica = new PersonaFisica();
            CopyCompany(company, fisica);
            fisica.MyApellidoPaterno = company.Op1;
            fisica.MyApellidoMaterno = company.Op2;
            return fisica;
        }

        private static void CopyCompany(Company source, Company dest)
        {
            dest.Code = source.Code;
            dest.CompanyName = source.CompanyName;
            dest.CompanyType = source.CompanyType;
            dest.IdType = source.IdType;
            dest.NumberId = source.NumberId;
            dest.Op1 = source.Op1;
            dest.Op2 = source.Op2;
            dest.Address = source.Address;
            dest.Mail = source.Mail;
            dest.PhoneNumber1 = source.PhoneNumber1;
            dest.PhoneNumber2 = source.PhoneNumber2;
            dest.Notes = source.Notes;
            dest.WebSite = source.WebSite;
            dest.MoneyType = source.MoneyType;
            dest.CreatedBy = source.CreatedBy;
            dest.UpdatedBy = source.UpdatedBy;
            dest.Active = source.Active;
            dest.CreatedAt = source.CreatedAt;
            dest.UpdateAt = source.UpdateAt;
            dest.CopyFrom = source.CopyFrom;
        }

        private IEnumerable<Account> LoadAccountsForNewCompany(Company company)
        {
            var copyFrom = company.CopyFrom;
            if (string.IsNullOrWhiteSpace(copyFrom) || copyFrom == "POR DEFECTO")
            {
                var defaults = _unitOfWork.AccountRepository.GetDefaultAccounts()
                    .Where(a => a.Id <= DefaultChartOfAccounts.AccountCount)
                    .ToList();
                foreach (var account in defaults)
                {
                    account.CompanyId = company.Code;
                    if (account.UpdatedBy == 0)
                        account.UpdatedBy = company.CreatedBy;
                }
                return defaults;
            }

            var source = _unitOfWork.AccountRepository.FindByCompanyId(copyFrom).ToList();
            if (source.Count == 0)
                throw new InvalidOperationException("No se pudo clonar el maestro de cuentas");

            return source.Select(a => CloneAccount(a, company.Code)).ToList();
        }

        private static Account CloneAccount(Account source, string newCompanyId)
        {
            return new Account
            {
                Id = source.Id,
                Name = source.Name,
                Memo = source.Memo,
                Editable = source.Editable,
                AccountTag = source.AccountTag,
                AccountType = source.AccountType,
                CompanyId = newCompanyId,
                PathDirection = source.PathDirection,
                FatherAccount = source.FatherAccount,
                DebOCred = source.DebOCred,
                PriorBalance = source.PriorBalance,
                PriorBalanceForeign = source.PriorBalanceForeign,
                DebitBalance = source.DebitBalance,
                DebitBalanceForeign = source.DebitBalanceForeign,
                CreditBalance = source.CreditBalance,
                CreditBalanceForeign = source.CreditBalanceForeign,
                CreatedBy = source.CreatedBy,
                UpdatedBy = source.UpdatedBy,
                Active = source.Active
            };
        }
    }
}
