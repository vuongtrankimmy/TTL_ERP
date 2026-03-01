
const axios = require('axios');

const CORE_API = 'http://localhost:5043/api/v1';

async function debugData() {
    console.log('--- DEBUGGING DATA ---');
    try {
        const res = await axios.get(`${CORE_API}/employees/debug-list`);
        console.log("Employees in DB:", JSON.stringify(res.data, null, 2));
    } catch (e) {
        console.error("Debug failed:", e.message);
    }
}

debugData();
