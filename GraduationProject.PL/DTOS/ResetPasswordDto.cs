﻿namespace GraduationProject.API.DTOS
{
    public class ResetPasswordDto
    {
        public string NewPassword { get; set; }

        public string NewConfirmPassword { get; set; }

        public string Email { get; set; }
        public string Token { get; set; }
    }
}
