<script setup>
import { ref, onMounted } from 'vue'

  const documents = ref([])
  const defaultFilter = {
    title: "",
    description: "",
    responsibleUnit: "",
    createdAt: null,
    url: "",
    fileType: "",
    estimatedReadingTimeMinutes: null,
    importanceLevel: "",
    category: "",
    isActive: null
  }

  const filters = ref(structuredClone(defaultFilter))

  const buildQueryParams = (obj) => {
    const params = new URLSearchParams();

    Object.entries(obj).forEach(([key, value]) => {
      if (value !== null && value !== "") {
        params.append(key, value);
      }
    });

    return params.toString();
  };

  const load = async () => {
    const query = buildQueryParams(filters.value);

    const res = await fetch(`https://localhost:5000/api/documents?${query}`)
    documents.value = await res.json()
  }

  const clear = async() => {
    filters.value = structuredClone(defaultFilter);
    load();
  }

  const selectedFile = ref(null);
  const onFileSelected = (event) => {
    selectedFile.value = event.target.files[0];
  };

  const importXml = async () => {
    if (!selectedFile.value) {
      alert("Please select an XML file first");
      return;
    }

    const formData = new FormData();
    formData.append("file", selectedFile.value);

    try{
      const res = await fetch("https://localhost:5000/api/import-xml", {
        method: "POST",
        body: formData
      })

      if (!res.ok) {
        throw new Error("Imports neizdevās!");
      }

      const data = await res.json();

      console.log(`Importēti ${data.imported} dokumenti. Atjaunoti ${data.updated} dokumenti`);

      load();
    }
    catch(err) {
      console.error("Kļūda importējot XML:", err);
    };


    load();
};

  onMounted(load);

</script>

<template>
  <div class="app">
    <h1>Dokumenti</h1>
    <div style="margin:5px">
      <input id="file_select" type="file" @change="onFileSelected" accept=".xml" />
      <button @click="importXml" class="import-btn">
        Importēt XML
      </button>
    </div>

    <div class="filter-grid">
      <div>
        <label for="filter-title">Nosaukums</label>
        <input id="filter-title" type="text" v-model="filters.title"/>
      </div>

      <div>
        <label for="filter-description">Apraksts</label>
        <input id="filter-description" type="text" v-model="filters.description"/>
      </div>

      <div>
        <label for="filter-responsible-unit">Struktūrvienība</label>
        <input id="filter-responsible-unit" type="text" v-model="filters.responsibleUnit"/>
      </div>

      <div>
        <label for="filter-created-at">Datums</label>
        <input id="filter-created-at" type="date" v-model="filters.createdAt"/>
      </div>

      <div>
        <label for="filter-url">Saite</label>
        <input id="filter-url" type="url" v-model="filters.url"/>
      </div>

      <div>
        <label for="filter-file-type">Faila tips</label>
        <input id="filter-file-type" type="text" v-model="filters.fileType"/>
      </div>

      <div>
        <label for="filter-reading-time">Aptuvenais lasīšanas laiks</label>
        <input id="filter-reading-time" type="number" min="0" v-model="filters.estimatedReadingTimeMinutes"/>
      </div>

      <div>
        <label for="filter-importance">Svarīgums</label>
        <input id="filter-importance" type="text" v-model="filters.importanceLevel"/>
      </div>

      <div>
        <label for="filter-category">Kategorija</label>
        <input id="filter-category" type="text" v-model="filters.category"/>
      </div>

      <div>
        <label for="filter-is-active">Stāvoklis</label>
        <select id="filter-is-active" v-model="filters.isActive">
          <option :value="null">Visi</option>
          <option :value="true">Aktīvs</option>
          <option :value="false">Neaktīvs</option>
        </select>
      </div>
    </div>
    <div style="margin:5px">
      <input type="button" @click="load" value="Atlasīt"/>
      <input type="button" @click="clear" value="Notīrīt filtrus"/>
    </div>


    <table>
      <thead>
        <tr>
          <th>Nosaukums</th>
          <th>Apraksts</th>
          <th>Struktūrvienība</th>
          <th>Datums</th>
          <th>Saite</th>
          <th>Tips</th>
          <th>Aptuvenais lasīšanas laiks (minūtes)</th>
          <th>Svarīgums</th>
          <th>Kategorija</th>
          <th>Aktīvs</th>
        </tr>
      </thead>

      <tbody>
        <tr v-for="doc in documents" :key="doc.id">
          <td>{{ doc.title }}</td>
          <td>{{ doc.description }}</td>
          <td>{{ doc.responsibleUnit }}</td>
          <td>{{ doc.createdAt }}</td>
          <td><a :href="doc.url" target="_blank">{{doc.url}}</a></td>
          <td>{{ doc.fileType }}</td>
          <td>{{ doc.estimatedReadingTimeMinutes }}</td>
          <td>{{ doc.importanceLevel }}</td>
          <td>{{ doc.category }}</td>
          <td>{{ doc.isActive ? 'Jā' : 'Nē' }}</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>



<style>
.app {
  max-width: 1200px;
  margin: 0 auto;
  padding: 1rem;
}
.filters {
  display: flex;
  gap: 1rem;
  margin-bottom: 1rem;
}
table {
  width: 100%;
  border-collapse: collapse;
}
th, td {
  border: 1px solid #ddd;
  padding: 0.5rem;
}
th {
  cursor: pointer;
  background: #f5f5f5;
}

.filter-grid {
  display: flex;
  flex-wrap: wrap;
  gap: 1rem;
  margin:5px;
}

.filter-grid > div {
  display: flex;
  flex-direction: column;
  flex: 1 1 200px;
}

.filter-grid label {
  font-size: 0.875rem;
  font-weight: 600;
  margin-bottom: 0.25rem;
}

.filter-grid input {
  padding: 0.5rem;
  border: 1px solid #ccc;
  border-radius: 4px;
  box-sizing: border-box;
}
</style>
