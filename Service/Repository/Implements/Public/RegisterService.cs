using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.View.Public;
using Service.Repository.Interfaces.Public;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Filter;

namespace Service.Repository.Implements.Public
{
    /// <summary>
    /// 注册
    /// </summary>
    public class RegisterService : DbContext, IRegisterService
    {
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="register">注册信息</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<bool> RegisterAsync(Register register)
        {
            //查询短信验证码
            var getCode = await Db.Queryable<p_sms_send_record>().Where(a => a.phone_no == register.phone_no && a.scene == "用户注册验证码" && a.expire_time > DateTime.Now).OrderBy(a => a.expire_time, SqlSugar.OrderByType.Desc).FirstAsync();

            if (getCode == null)
            {
                throw new MessageException("请重新获取验证码!");
            }

            //手机验证码
            if (register.code != getCode.code && !string.IsNullOrEmpty(register.code))
            {
                throw new MessageException("验证码错误!");
            }

            var existOrg = await Db.Queryable<p_org>().Where(w => w.name == register.name || w.phone_no == register.phone_no).WithCache().FirstAsync();
            if (existOrg != null)
            {
                if (existOrg.name == register.name)
                {
                    throw new MessageException("集团名称已存在，请确认后重新注册");
                }
                else
                {
                    throw new MessageException("当前联系人手机号已注册集团，请更换手机号后重新注册");
                }
            }

            var result = Db.Ado.UseTran(() =>
            {
                #region 集团信息

                var org = new p_org();
                org.city_code = register.city_code;
                org.city_name = register.city_name;

                var code = "00001";
                //获取集团最大编码
                var maxCode = Db.Queryable<p_org>().WithCache().Max(m => m.code);
                if (!string.IsNullOrEmpty(maxCode))
                {
                    code = (int.Parse(maxCode) + 1).ToString().PadLeft(5, '0');
                }

                org.code = code;
                org.city_code = register.city_code;
                org.city_name = register.city_name;
                org.county_code = register.county_code;
                org.county_name = register.county_name;
                org.create_time = DateTime.Now;
                org.expire_time = DateTime.Now.AddDays(int.Parse(ConfigExtensions.Configuration["BaseConfig:ExpireDay"]));
                org.legal_manager = register.legal_manager;
                org.legal_phone_no = register.legal_phone_no;
                org.license_no = register.license_no;
                org.license_path = register.license_path;
                org.link_man = register.link_man;
                org.name = register.name;
                org.phone_no = register.phone_no;
                org.pinyin = ToSpell.GetFirstPinyin(org.name);
                org.province_code = register.province_code;
                org.province_name = register.province_name;
                org.reg_address = register.reg_address;
                org.status = 1;
                org.street_code = register.street_code;
                org.street_name = register.street_name;
                org.office_address = register.reg_address;
                org.integral_card = 2;
                org.physical_card = 3;

                var org_id = Db.Insertable(org).ExecuteReturnIdentity();
                redisCache.RemoveAll<p_org>();

                #endregion

                #region 人员信息

                var employee = new p_employee();
                employee.org_id = org_id;
                employee.name = org.link_man;
                employee.pinyin = ToSpell.GetFirstPinyin(org.link_man);
                employee.phone_no = org.phone_no;
                employee.password = MetarnetRegex.Encrypt(org.phone_no.Substring(org.phone_no.Length - 6));
                employee.is_admin = true;
                employee.create_time = DateTime.Now;
                employee.expire_time = org.expire_time;

                var employee_id = Db.Insertable(employee).ExecuteReturnIdentity();
                redisCache.RemoveAll<p_employee>();

                #endregion

                #region 人员角色

                var p_employee_role = new p_employee_role()
                {
                    dept_id = 0,
                    employee_id = employee_id,
                    is_admin = true,
                    org_id = org_id,
                    role_id = 0,
                    store_id = 0
                };

                Db.Insertable(p_employee_role).RemoveDataCache().ExecuteCommand();
                #endregion
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }
    }
}
