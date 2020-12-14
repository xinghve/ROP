using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    public class HomeModel
    {
        private System.String _name;
        /// <summary>
        /// 姓名
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.Int16? _age;
        /// <summary>
        /// 年龄
        /// </summary>
        public System.Int16? age { get { return this._age; } set { this._age = value ?? default(System.Int16); } }

        private System.String _phone;
        /// <summary>
        /// 联系电话
        /// </summary>
        public System.String phone { get { return this._phone; } set { this._phone = value?.Trim(); } }

        private System.Int32 _id;
        /// <summary>
        /// 主键
        /// </summary>
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.String _sex;
        /// <summary>
        /// 性别
        /// </summary>
        public System.String sex { get { return this._sex; } set { this._sex = value?.Trim(); } }
        /// <summary>
        /// 是否已读
        /// </summary>
        public bool isRead { get; set; }

        private System.String _image_url;
        /// <summary>
        /// 头像图片地址
        /// </summary>
        public System.String image_url { get { return this._image_url; } set { this._image_url = value?.Trim(); } }

        /// <summary>
        /// 健康管家id
        /// </summary>
        public int to_director_id { get; set; }

        /// <summary>
        /// 健康管家
        /// </summary>
        public string to_director { get; set; }
    }

    /// <summary>
    /// 修改节日已读
    /// </summary>
    public class UpdateModel
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public int id { get; set; }
    }

    /// <summary>
    /// 随访model
    /// </summary>
    public class FollowUpModel
    {
        private System.String _name;
        /// <summary>
        /// 姓名
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.Int32 _id;
        /// <summary>
        /// 随访id
        /// </summary>
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        /// <summary>
        /// 是否已读
        /// </summary>
        public bool isRead { get; set; }


        private System.String _image_url;
        /// <summary>
        /// 头像图片地址
        /// </summary>
        public System.String image_url { get { return this._image_url; } set { this._image_url = value?.Trim(); } }
    }

    /// <summary>
    /// 余额下限
    /// </summary>
    public class BalanceLowerModel
    {
        private System.String _name;
        /// <summary>
        /// 姓名
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.Int32 _id;
        /// <summary>
        /// 随访id
        /// </summary>
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        /// <summary>
        /// 是否已读
        /// </summary>
        public bool isRead { get; set; }

        private System.String _image_url;
        /// <summary>
        /// 头像图片地址
        /// </summary>
        public System.String image_url { get { return this._image_url; } set { this._image_url = value?.Trim(); } }
    }

}
