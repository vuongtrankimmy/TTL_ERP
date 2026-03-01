const axios = require('axios');

const CORE_API = 'http://localhost:5043/api/v1';
const AUTH_API = 'http://localhost:5272/api/v1';

async function getDetail() {
    try {
        const loginRes = await axios.post(AUTH_API + '/auth/login', { username: 'admin', password: 'admin123' });
        const token = loginRes.data.data.accessToken;
        const authHeader = { headers: { Authorization: 'Bearer ' + token } };

        const periodsRes = await axios.get(CORE_API + '/payroll/periods?month=2&year=2026', authHeader);
        const periodList = periodsRes.data.data.items || periodsRes.data.data || [];
        const periodId = periodList[0].id;

        const detailRes = await axios.get(CORE_API + '/payroll/periods/' + periodId + '/detail?searchTerm=GD-001&pageSize=100', authHeader);
        console.log(JSON.stringify(detailRes.data.data.payrolls.items[0], null, 2));
    } catch (err) {
        console.error(err.message);
    }
}
getDetail();
