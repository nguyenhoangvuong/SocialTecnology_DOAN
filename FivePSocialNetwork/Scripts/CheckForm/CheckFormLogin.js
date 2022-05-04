$('.ui.form')
    .form({
        fields: {
            user_email: {
                identifier: 'user_email',
                rules: [
                    {
                        type: 'empty',
                        prompt: 'Vui lòng nhập email or Số điện thoại !'
                    }
                ]
            },
            user_pass: {
                identifier: 'user_pass',
                rules: [
                    {
                        type: 'empty',
                        prompt: 'Vui lòng nhập password!'
                    }
                ]
            },
        },
        inline: true,
        on: 'blur'
    })
    ;