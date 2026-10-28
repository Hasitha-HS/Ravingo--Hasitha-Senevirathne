import React, { useState, useEffect } from "react";
import axios from "axios";

const API_BASE = "http://localhost:5000/api";

export default function App() {
  const [branches, setBranches] = useState([]);
  const [selectedBranch, setSelectedBranch] = useState(null);
  const [stock, setStock] = useState([]);
  const [transfers, setTransfers] = useState([]);
  
  const [fromBranchId, setFromBranchId] = useState("");
  const [toBranchId, setToBranchId] = useState("");
  const [itemId, setItemId] = useState("");
  const [quantity, setQuantity] = useState(1);
  const [message, setMessage] = useState("");

  useEffect(() => {
    fetchBranches();
    fetchTransfers();
  }, []);

  const fetchBranches = async () => {
    try {
      const res = await axios.get(`${API_BASE}/branches`);
      setBranches(res.data);
    } catch (err) {
      console.error(err);
    }
  };

  const fetchTransfers = async () => {
    try {
      const res = await axios.get(`${API_BASE}/transfers`);
      setTransfers(res.data);
    } catch (err) {
      console.error(err);
    }
  };

  const loadBranchStock = async (branchId) => {
    setSelectedBranch(branchId);
    try {
      const res = await axios.get(`${API_BASE}/branches/${branchId}/stock`);
      setStock(res.data);
    } catch (err) {
      console.error(err);
    }
  };

  const handleCreateTransfer = async (e) => {
    e.preventDefault();
    setMessage("");
    try {
      await axios.post(`${API_BASE}/transfers`, {
        fromBranchId: parseInt(fromBranchId),
        toBranchId: parseInt(toBranchId),
        items: [{ itemId: parseInt(itemId), quantity: parseInt(quantity) }]
      });
      setMessage("Transfer requested successfully!");
      fetchTransfers();
    } catch (err) {
      setMessage(err.response?.data?.message || "Error creating transfer");
    }
  };

  const handleAction = async (id, action) => {
    setMessage("");
    try {
      await axios.post(`${API_BASE}/transfers/${id}/${action}`);
      setMessage(`Transfer successfully ${action}ed!`);
      fetchTransfers();
      if (selectedBranch) loadBranchStock(selectedBranch);
    } catch (err) {
      setMessage(err.response?.data?.message || `Error during ${action}`);
    }
  };

  return (
    <div style={{ padding: "20px", fontFamily: "Arial, sans-serif", maxWidth: "1000px", margin: "auto" }}>
      <h2>Ravigno Stock Transfer Portal</h2>
      {message && <div style={{ padding: "10px", background: "#eee", marginBottom: "15px" }}>{message}</div>}

      <div style={{ display: "flex", gap: "20px" }}>
        <div style={{ flex: 1 }}>
          <h3>Branches & Warehouses</h3>
          <ul>
            {branches.map(b => (
              <li key={b.id} style={{ marginBottom: "8px" }}>
                <b>{b.name}</b> ({b.location}) 
                <button onClick={() => loadBranchStock(b.id)} style={{ marginLeft: "10px" }}>View Stock</button>
              </li>
            ))}
          </ul>

          {selectedBranch && (
            <div>
              <h4>Stock for Branch #{selectedBranch}</h4>
              <table border="1" cellPadding="5" style={{ width: "100%", borderCollapse: "collapse" }}>
                <thead>
                  <tr><th>SKU</th><th>Item</th><th>Qty</th></tr>
                </thead>
                <tbody>
                  {stock.map(s => (
                    <tr key={s.itemId}>
                      <td>{s.sku}</td>
                      <td>{s.itemName}</td>
                      <td>{s.quantity}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>

        <div style={{ flex: 1 }}>
          <h3>Request Transfer</h3>
          <form onSubmit={handleCreateTransfer} style={{ display: "flex", flexDirection: "column", gap: "10px" }}>
            <label>From Branch ID: <input value={fromBranchId} onChange={e => setFromBranchId(e.target.value)} /></label>
            <label>To Branch ID: <input value={toBranchId} onChange={e => setToBranchId(e.target.value)} /></label>
            <label>Item ID: <input value={itemId} onChange={e => setItemId(e.target.value)} /></label>
            <label>Quantity: <input type="number" value={quantity} onChange={e => setQuantity(e.target.value)} /></label>
            <button type="submit">Submit Transfer Request</button>
          </form>
        </div>
      </div>

      <hr style={{ margin: "30px 0" }} />

      <h3>Active Transfers</h3>
      <table border="1" cellPadding="6" style={{ width: "100%", borderCollapse: "collapse" }}>
        <thead>
          <tr>
            <th>ID</th><th>From</th><th>To</th><th>Status</th><th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {transfers.map(t => (
            <tr key={t.id}>
              <td>{t.id}</td>
              <td>{t.fromBranchId}</td>
              <td>{t.toBranchId}</td>
              <td><b>{t.status}</b></td>
              <td>
                {t.status === "Requested" && (
                  <>
                    <button onClick={() => handleAction(t.id, "dispatch")}>Dispatch</button>
                    <button onClick={() => handleAction(t.id, "cancel")} style={{ marginLeft: "5px" }}>Cancel</button>
                  </>
                )}
                {t.status === "Dispatched" && (
                  <button onClick={() => handleAction(t.id, "receive")}>Receive</button>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
