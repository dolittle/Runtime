const path = require('path');
const envPath = path.resolve(process.cwd(), 'dolittle.env');

require('dotenv').config({ path: envPath });

const webpack = require('@dolittle/typescript.webpack.aurelia').webpack
const originalConfig = webpack(__dirname);

console.log(process.env.DOLITTLE_WEB_TITLE);

module.exports = () => {
    const config = originalConfig.apply(null, arguments);
    config.devServer = {
        historyApiFallback: true,
        port: 8080,
        proxy: {
          '/api': 'http://localhost:5000',
          '/swagger': 'http://localhost:5000'
        }
      };
    return config;    
};
