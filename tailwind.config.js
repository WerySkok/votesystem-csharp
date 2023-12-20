/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        './Pages/**/*.cshtml',
        './Views/**/*.cshtml',
        'static/js/preline.min.js',
    ],
    darkMode: 'media',
    theme: {
        fontFamily: {
            sans: ['Inter', 'sans-serif'],
        },
        extend: {},
    },
    plugins: [require('preline/plugin')],
};
