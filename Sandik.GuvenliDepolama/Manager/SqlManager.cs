﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using System.Data.SqlClient;
using Sandik.GuvenliDepolama.Models;

namespace Sandik.GuvenliDepolama.Manager
{
    public class SqlManager
    {
        public string AddUser(User item)
        {
            var ret = "";
            try
            {
                using (var connection = new SqlConnection(Setting.ConnectionString))
                {

                    var param = new DynamicParameters();
                    param.Add("Name", item.NameEnc);
                    param.Add("Surname", item.SurNameEnc);
                    param.Add("Mail", item.Mail);
                    param.Add("MailEnc", item.MailEnc);
                    param.Add("Password", item.Password);
                    ret = connection.Query<string>("CreateUser", param: param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();

                }
            }
            catch (Exception)
            {
                throw;
            }
            return ret;
        }

        public User CheckUser(string Mail, string Password,string SecureCode)
        {
            var ret = new User();
            try
            {
                using (var connection = new SqlConnection(Setting.ConnectionString))
                {
                    var param = new DynamicParameters();
                    param.Add("Mail", Mail);
                    param.Add("Password", Password);
                    param.Add("SecureCode", SecureCode);

                    ret = connection.Query<User>("dbo.CheckUser", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Login işleminde hata", ex);
            }
            return ret;
        }

        public string CheckSecureCode(string Mail,string SecureCode)
        {
            var ret = "";
            try
            {
                using (var connection = new SqlConnection(Setting.ConnectionString))
                {
                    var param = new DynamicParameters();
                    param.Add("Mail", Mail);
                    param.Add("SecureCode", SecureCode);                    

                    var retTwoFactor = connection.Query<TwoFactor>("dbo.CheckTwoFactor", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                    if (retTwoFactor == null)
                    {
                        ret = "Kod geçersiz. Lütfen tekrar deneyin.";
                    }
                    else if (retTwoFactor.LastTimeMinute > Setting.TwoFactorTimeOut)
                    {
                        ret = "Doğrulama kodunuz zaman aşımına uğradı, lütfen yeni kod alın.";
                    }
                }                
            }
            catch (Exception ex)
            {
                ret = ex.Message;
                throw new Exception("İki aşamalı kod sqlden kontrol edilirken hata oluştu.",ex);
            }
            return ret;
        }

        public bool SetUserFile(UserFile fileInfo)
        {
            var ret = true;
            try
            {
                using (var connection = new SqlConnection(Setting.ConnectionString))
                {
                    var param = new DynamicParameters();
                    param.Add("UserID", fileInfo.UserID);
                    param.Add("FileNameGuid", fileInfo.FileNameGuid);
                    param.Add("FileNameOrjinal", fileInfo.FileNameOrjinal);
                    param.Add("FilePath", fileInfo.FilePath);
                    param.Add("FileSizeByte", fileInfo.FileSizeByte);

                    ret = connection.Execute("dbo.SetUserFile", param, commandType: System.Data.CommandType.StoredProcedure) > 0;                    
                }

            }
            catch (Exception ex)
            {
                ret = false;
                throw new Exception("Dosya bilgisi dbye kayıt edilirken hata oldu.", ex);
            }
            return ret;
        }

        public bool SetUserFileDelete(UserFile fileInfo)
        {
            var ret = true;
            try
            {
                using (var connection = new SqlConnection(Setting.ConnectionString))
                {
                    var param = new DynamicParameters();
                    param.Add("UserID", fileInfo.UserID);
                    param.Add("ID", fileInfo.ID);
                    param.Add("IsDeleted", fileInfo.IsDeleted);                  

                    ret = connection.Execute("dbo.SetUserFile", param, commandType: System.Data.CommandType.StoredProcedure) > 0;
                }

            }
            catch (Exception ex)
            {
                ret = false;
                throw new Exception("Dosya bilgisi dbden silinirken hata oldu.", ex);
            }
            return ret;
        }

        public List<UserFile> GetUserFile(int UserID,string FileNameOriginal="")
        {
            List<UserFile> items = new List<UserFile>();
            try
            {
                using (var connection = new SqlConnection(Setting.ConnectionString))
                {
                    var param = new DynamicParameters();
                    param.Add("UserID", UserID);
                    param.Add("FileName", FileNameOriginal);

                   items = connection.Query<UserFile>("dbo.GetUserFile", param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Dosya bilgisi çekilemedi", ex);
            }
            return items;
        }
    }
}