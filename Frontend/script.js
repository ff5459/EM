const apiUrl = 'http://localhost/api/v1/orders';

// Upload CSV file
async function uploadFile() {
  const fileInput = document.getElementById('fileInput');
  const uploadMessage = document.getElementById('uploadMessage');

  if (!fileInput.files[0]) {
    uploadMessage.textContent = "Please select a CSV file to upload.";
    return;
  }

  const formData = new FormData();
  formData.append('file', fileInput.files[0]);

  const response = await fetch(`${apiUrl}/upload`, {
    method: 'POST',
    body: formData,
  });

  if (!response.ok) {
    uploadMessage.textContent = await response.text();
    uploadMessage.style.color = 'red';
    return;
  }

  uploadMessage.textContent = "File uploaded successfully!";
  uploadMessage.style.color = 'green';
}

// Fetch orders based on region
async function fetchOrders() {
  const region = document.getElementById('regionInput').value;
  const fetchMessage = document.getElementById('fetchMessage');

  if (!region) {
    fetchMessage.textContent = "Please enter a region.";
    return;
  }

  try {
    const response = await fetch(`${apiUrl}?region=${encodeURIComponent(region)}`);
    const orders = await response.json();

    if (orders.length > 0) {
      displayOrders(orders);
      fetchMessage.textContent = "";
    } else {
      fetchMessage.textContent = "No orders found for the specified region.";
    }
  } catch (error) {
    console.error("Error fetching orders:", error);
    fetchMessage.textContent = "An error occurred while fetching orders.";
  }
}

// Download orders as CSV based on region
async function downloadOrders() {
  const region = document.getElementById('regionInput').value;
  const fetchMessage = document.getElementById('fetchMessage');

  if (!region) {
    fetchMessage.textContent = "Please enter a region.";
    return;
  }

  try {
    const response = await fetch(`${apiUrl}/download?region=${encodeURIComponent(region)}`, {
      method: 'GET',
    });

    if (response.ok) {
      const blob = await response.blob();
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = `orders_${region}.csv`;
      document.body.appendChild(link);
      link.click();
      link.remove();
      fetchMessage.textContent = "";
    } else {
      console.log(response.status);
      fetchMessage.textContent = "No orders found for download.";
    }
  } catch (error) {
    console.error("Error downloading orders:", error);
    fetchMessage.textContent = "An error occurred while downloading the CSV.";
  }
}

// Display orders in the table
function displayOrders(orders) {
  const tableBody = document.getElementById('ordersTableBody');
  tableBody.innerHTML = '';

  orders.forEach((order) => {
    const row = document.createElement('tr');

    const orderIdCell = document.createElement('td');
    orderIdCell.textContent = order.id;
    row.appendChild(orderIdCell);

    const dateCell = document.createElement('td');
    dateCell.textContent = new Date(order.date).toLocaleString();
    row.appendChild(dateCell);

    const regionCell = document.createElement('td');
    regionCell.textContent = order.region;
    row.appendChild(regionCell);

    tableBody.appendChild(row);
  });
}

async function deleteData() {
  const uploadMessage = document.getElementById('uploadMessage');
  try {
    const responce = await fetch(`${apiUrl}`, {
      method: 'DELETE'
    });
    uploadMessage.textContent = 'File deleted successfully!';
    uploadMessage.style.color = 'green';
  }
  catch (error) {
    uploadMessage.textContent = error;
    uploadMessage.style.color = 'red'
  }
}