/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        './Pages/**/*.cshtml',
        './Views/**/*.cshtml',
        'wwwroot/js/preline.min.js',
    ],
    darkMode: 'media',
    theme: {
        fontFamily: {
            sans: ['Inter', 'sans-serif'],
        },
        extend: {},
    },
    plugins: [require('@tailwindcss/forms'), require('preline/plugin')],
};
