$('.ui.form')
    .form({
        fields: {
            user_pass: {
                identifier: 'user_pass',
                rules: [
                    {
                        type: 'empty',
                        prompt: 'Vui lòng nhập mật khẩu!'
                    },
                    {
                        type: 'regExp',
                        value: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/,
                        prompt: 'Mật khẩu phải có ký tự in hoa, ký tự thường, ký tự số, và ký tự đặc biệt!'
                    }
                ]
            },
            newPass: {
                identifier: 'newPass',
                rules: [
                    {
                        type: 'empty',
                        prompt: 'Vui lòng nhập mật khẩu!'
                    },
                    {
                        type: 'minLength[5]',
                        prompt: 'Vui lòng nhập mật khẩu hợp lệ!'
                    },
                    {
                        type: 'regExp',
                        value: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/,
                        prompt: 'Mật khẩu phải có ký tự in hoa, ký tự thường, ký tự số, và ký tự đặc biệt!'
                    }
                ]
            },
            retypePass: {
                identifier: 'retypePass',
                rules: [
                    {
                        type: 'empty',
                        prompt: 'Vui lòng xác nhập mật khẩu!'
                    },
                    {
                        type: 'match[newPass]',
                        prompt: 'Mật khẩu xác nhập không trùng khớp!'
                    }
                ]
            }
        },
        inline: true,
        on: 'blur'
    })
    ;