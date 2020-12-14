using Models.DB;
using Models.View.Public;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Crm
{
    /// <summary>
    /// 会员卡分页实体
    /// </summary>
     public class CardPageModel: c_account
    {
        private System.String _archives;
        /// <summary>
        /// 档案姓名
        /// </summary>
        public System.String archives { get { return this._archives; } set { this._archives = value?.Trim(); } }

        private System.String _archives_phone;
        /// <summary>
        /// 档案手机
        /// </summary>
        public System.String archives_phone { get { return this._archives_phone; } set { this._archives_phone = value?.Trim(); } }

        private System.String _id_card;
        /// <summary>
        /// 证件号
        /// </summary>
        public System.String id_card { get { return this._id_card; } set { this._id_card = value?.Trim(); } }

        private System.Int32 _store_id;
        /// <summary>
        /// 门店id
        /// </summary>
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }

        private System.Int32 _level_id;
        /// <summary>
        /// 等级id
        /// </summary>
        public System.Int32 level_id { get { return this._level_id; } set { this._level_id = value; } }

        private System.String _level;
        /// <summary>
        /// 等级
        /// </summary>
        public System.String level { get { return this._level; } set { this._level = value?.Trim(); } }

        private System.String _card_no;
        /// <summary>
        /// 就诊卡号
        /// </summary>
        public System.String card_no { get { return this._card_no; } set { this._card_no = value?.Trim(); } }

        private System.String _virtual_no;
        /// <summary>
        /// 虚拟就诊卡号
        /// </summary>
        public System.String virtual_no { get { return this._virtual_no; } set { this._virtual_no = value?.Trim(); } }


        private System.String _creater;
        /// <summary>
        /// 创建人
        /// </summary>
        public System.String creater { get { return this._creater; } set { this._creater = value?.Trim(); } }

        private System.DateTime? _create_date;
        /// <summary>
        /// 创建时间
        /// </summary>
        public System.DateTime? create_date { get { return this._create_date; } set { this._create_date = value ?? default(System.DateTime); } }

        private System.String _store_name;
        /// <summary>
        ///门店名
        /// </summary>
        public System.String store_name { get { return this._store_name; } set { this._store_name = value?.Trim(); } }

        private System.String _sex;
        /// <summary>
        ///性别
        /// </summary>
        public System.String sex { get { return this._sex; } set { this._sex = value?.Trim(); } }

        /// <summary>
        /// 优惠券列表
        /// </summary>
        public List<ArchivesCardModel> couponList { get; set; }

        private System.Decimal? _balance_limit_lower;
        /// <summary>
        /// 余额下限
        /// </summary>
        public System.Decimal? balance_limit_lower { get { return this._balance_limit_lower; } set { this._balance_limit_lower = value ?? default(System.Decimal); } }

        private System.String _image_url;
        /// <summary>
        /// 头像图片地址
        /// </summary>
        public System.String image_url { get { return this._image_url; } set { this._image_url = value?.Trim(); } }

        private System.String _drink;
        /// <summary>
        /// 饮料
        /// </summary>
        public System.String drink { get { return this._drink; } set { this._drink = value?.Trim(); } }

        private System.String _fruits;
        /// <summary>
        /// 水果
        /// </summary>
        public System.String fruits { get { return this._fruits; } set { this._fruits = value?.Trim(); } }

        private System.String _foods;
        /// <summary>
        /// 饮食
        /// </summary>
        public System.String foods { get { return this._foods; } set { this._foods = value?.Trim(); } }

        private System.String _smoke;
        /// <summary>
        /// 吸烟
        /// </summary>
        public System.String smoke { get { return this._smoke; } set { this._smoke = value?.Trim(); } }

        private System.String _habit;
        /// <summary>
        /// 习惯
        /// </summary>
        public System.String habit { get { return this._habit; } set { this._habit = value?.Trim(); } }

        private System.String _hobby;
        /// <summary>
        /// 爱好
        /// </summary>
        public System.String hobby { get { return this._hobby; } set { this._hobby = value?.Trim(); } }
    }

    /// <summary>
    /// 领取优惠券实体
    /// </summary>
    public class ArchivesCardModel
    {

        private System.Int16? _state;
        /// <summary>
        /// 状态（2=已使用，3=未使用）
        /// </summary>
        public System.Int16? state { get { return this._state; } set { this._state = value ?? default(System.Int16); } }

        private System.Int32 _id;
        /// <summary>
        /// 主键
        /// </summary>
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.String _name;
        /// <summary>
        /// 名称
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.String _activityname;
        /// <summary>
        /// 名称
        /// </summary>
        public System.String activityname { get { return this._activityname; } set { this._activityname = value?.Trim(); } }


        private System.Decimal? _use_money;
        /// <summary>
        /// 使用金额
        /// </summary>
        public System.Decimal? use_money { get { return this._use_money; } set { this._use_money = value ?? default(System.Decimal); } }

        private System.Decimal? _deduction_money;
        /// <summary>
        /// 抵扣金额
        /// </summary>
        public System.Decimal? deduction_money { get { return this._deduction_money; } set { this._deduction_money = value ?? default(System.Decimal); } }

        private System.DateTime? _start_date;
        /// <summary>
        /// 开始时间
        /// </summary>
        public System.DateTime? start_date { get { return this._start_date; } set { this._start_date = value ?? default(System.DateTime); } }

        private System.DateTime? _end_date;
        /// <summary>
        /// 结束时间
        /// </summary>
        public System.DateTime? end_date { get { return this._end_date; } set { this._end_date = value ?? default(System.DateTime); } }

        /// <summary>
        /// 券码
        /// </summary>
        public string no { get; set; }
        /// <summary>
        /// 叠加
        /// </summary>
        public int? overlay { get; set; }

    }

    /// <summary>
    /// 消费记录
    /// </summary>
    public class ConsumerList
    {    
        /// <summary>
        /// 门店名
        /// </summary>
        public string store_name { get; set; }
        /// <summary>
        /// 档案id
        /// </summary>
        public int archives_id { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
        public string card_no { get; set; }
        /// <summary>
        /// 档案
        /// </summary>
        public string archives { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string archives_phone { get; set; }

        /// <summary>
        /// 消费时间
        /// </summary>
        public DateTime recharge_date { get; set; }

        /// <summary>
        /// 消费金额
        /// </summary>
        public Decimal recharge_money { get; set; }

        private System.String _way_code;
        /// <summary>
        /// 支付方式编码
        /// </summary>
        public System.String way_code { get { return this._way_code; } set { this._way_code = value?.Trim(); } }

        private System.String _way;
        /// <summary>
        /// 支付方式
        /// </summary>
        public System.String way { get { return this._way; } set { this._way = value?.Trim(); } }

        private System.String _bill_no;
        /// <summary>
        /// 流水号
        /// </summary>
        public System.String bill_no { get { return this._bill_no; } set { this._bill_no = value?.Trim(); } }

        private System.String _level;
        /// <summary>
        /// 会员等级
        /// </summary>
        public System.String level { get { return this._level; } set { this._level = value?.Trim(); } }

        private System.Int32? _store_id;
        /// <summary>
        /// 门店id
        /// </summary>
        public System.Int32? store_id { get { return this._store_id; } set { this._store_id = value ?? default(System.Int32); } }

        private System.Decimal? _balance;
        /// <summary>
        /// 余额
        /// </summary>
        public System.Decimal? balance { get { return this._balance; } set { this._balance = value ?? default(System.Decimal); } }

        private System.Int32 _creater_id;
        /// <summary>
        /// 创建人ID
        /// </summary>
        public System.Int32 creater_id { get { return this._creater_id; } set { this._creater_id = value; } }

        private System.String _creater;
        /// <summary>
        /// 创建人
        /// </summary>
        public System.String creater { get { return this._creater; } set { this._creater = value?.Trim(); } }

        private System.Int32? _consultation_id;
        /// <summary>
        /// 就诊id
        /// </summary>
        public System.Int32? consultation_id { get { return this._consultation_id; } set { this._consultation_id = value ?? default(System.Int32); } }

        private System.Int32? _check_id;
        /// <summary>
        /// 结账id
        /// </summary>
        public System.Int32? check_id { get { return this._check_id; } set { this._check_id = value ?? default(System.Int32); } }

        private System.Int16? _state_id;
        /// <summary>
        /// 状态id
        /// </summary>
        public System.Int16? state_id { get { return this._state_id; } set { this._state_id = value ?? default(System.Int16); } }

        private System.String _state;
        /// <summary>
        /// 状态
        /// </summary>
        public System.String state { get { return this._state; } set { this._state = value?.Trim(); } }

        private System.String _remark;
        /// <summary>
        /// 摘要
        /// </summary>
        public System.String remark { get { return this._remark; } set { this._remark = value?.Trim(); } }

      
        private System.Int32? _to_director_id;
        /// <summary>
        /// 现负责人ID
        /// </summary>
        public System.Int32? to_director_id { get { return this._to_director_id; } set { this._to_director_id = value ?? default(System.Int32); } }

        private System.String _to_director;
        /// <summary>
        /// 现负责人
        /// </summary>
        public System.String to_director { get { return this._to_director; } set { this._to_director = value?.Trim(); } }

        private System.String _coupon_no;
        /// <summary>
        /// 券码
        /// </summary>
        public System.String coupon_no { get { return this._coupon_no; } set { this._coupon_no = value?.Trim(); } }

        private System.Decimal _discount_rate;
        /// <summary>
        /// 折扣率
        /// </summary>
        public System.Decimal discount_rate { get { return this._discount_rate; } set { this._discount_rate = value; } }

      
    }

    /// <summary>
    /// 消费记录查询条件
    /// </summary>
    public class ConsumerSearch: Search
    {
        /// <summary>
        /// 门店id
        /// </summary>
        public int store_id { get; set; }

        /// <summary>
        /// 机构选择门店
        /// </summary>
        public int select_store_id { get; set; }

        /// <summary>
        /// 消费者/电话/创建者
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public string way_code { get; set; }

        /// <summary>
        /// 消费时间-开始
        /// </summary>
        
        private System.DateTime? _start_time;
        /// <summary>
        /// 开始时间
        /// </summary>
        public System.DateTime? start_time { get { return this._start_time; } set { this._start_time = value; } }

        private System.DateTime? _end_time;
        /// <summary>
        /// 结束时间
        /// </summary>
        public System.DateTime? end_time { get { return this._end_time; } set { this._end_time = value; } }
    }
}
