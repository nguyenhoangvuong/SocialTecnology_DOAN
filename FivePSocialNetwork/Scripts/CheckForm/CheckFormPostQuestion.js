$('.ui.form')
    .form({
        fields: {
            question_title: {
                identifier: 'question_title',
                rules: [
                    {
                        type: 'empty',
                        prompt: 'Vui lòng nhập tiêu đề câu hỏi!'
                    }
                ]
            },
            question_content: {
                identifier: 'question_content',
                rules: [
                    {
                        type: 'empty',
                        prompt: 'Vui lòng nhập nội dung!'
                    },
                    {
                        type: 'minLength[15]',
                        prompt: 'Nội dung câu hỏi tối thiểu 15 ký tự!'
                    }
                ]
            },
            tagsQuestion: {
                identifier: 'tagsQuestion',
                rules: [
                    {
                        type: 'empty',
                        prompt: 'Vui lòng nhập thẻ tags!'
                    },
                    {
                        type: 'minLength[15]',
                        prompt: 'Nội dung câu hỏi tối thiểu 15 ký tự!'
                    }
                ]
            },
            technologyQuestion: {
                identifier: 'technologyQuestion',
                rules: [
                    {
                        type: 'empty',
                        prompt: 'Vui lòng chọn công nghệ của câu hỏi!'
                    },
                    {
                        type: 'minLength[15]',
                        prompt: 'Nội dung câu hỏi tối thiểu 15 ký tự!'
                    }
                ]
            },
        },
        inline: true,
        on: 'blur'
    })
    ;