﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MayNghien.Common.CommonMessage
{
    public static class AuthResponseMessage
    {
        public const string ERR_MSG_UserNotFound = "Thông tin đăng nhập không chính xác";
        public const string ERR_MSG_UserNotConFirmed = "Tài khoản chưa được kích hoạt";
        public const string ERR_MSG_UserLockedOut = "Tài khoản đã bị khóa";
        public const string ERR_MSG_UserExisted = "Email này đã tồn tại";
        public const string ERR_MSG_EmailIsNullOrEmpty = "Phải nhập email";
        public const string ERR_MSG_RoleIsNullOrEmpty = "Phải nhập quyền";
        public const string INFO_MSG_UserCreated = "Tạo tài khoản thành công, vui xòng chờ admin kích hoạt";
        public const string INFO_MSG_UserDeleted = "Xóa tài khoản thành công";
        public const string ERR_MSG_RoleNotFound = "Quyền của người dùng không tồn tại";
        public const string ERR_MSG_NotHavePermision = "Bạn không đủ quyền để thực hiện thao tác này";
        public const string ERR_MSG_CanNotCreateUser = "Không thể tạo tài khoản";
    }
}
