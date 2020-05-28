<template>
    <main class="main">
        <section class="search-wrap">
            <Search v-bind:forceUpdate="forceUpdate" v-bind:selectedMedicaments="selectedMedicaments"/>
        </section>
        
        <div class="controls">
            <Settings v-bind:settings="settings"/>
            
            <button class="search-btn" v-on:click="sendMedicaments" :disabled="selectedMedicaments.size === 0">Искать</button>
        </div>
        

        <div class="selected-medicament-list-wrap">
            <SelectedMedicament
                    v-for="[key,value] in selectedMedicaments"
                    v-bind:key="key"
                    :medicament="value"
                    :deleteMedicament="() => deleteMedicament(key)"
            />
        </div>

        <div class="vld-parent">
            <VueLoading :active.sync="isLoading"
                     :can-cancel="false"
                     :is-full-page="true">
            </VueLoading>
        </div>
    </main>
    
</template>

<script>
    import VueLoading from 'vue-loading-overlay';
    import 'vue-loading-overlay/dist/vue-loading.css';
    import Settings from './Settings';
    import Search from "./Search";
    import SelectedMedicament from "./SelectedMedicament";
    
    
    export default {
        name: "Index",
        components: {SelectedMedicament, Search, Settings, VueLoading },
        props: {
            selectedMedicaments: {
                type: Map,
                default: () => new Map()
            },
            settings: {
                type: Object,
                default: () => ({
                    searchRadius : 1000,
                    useSearchRadius : false,
                    pharmaciesCount : 3,
                    limitedPharmaciesCount: false,
                    latitude: 1.5,
                    longitude: 1.5
                })
            }
        },
        data: function () {
            return {
                isLoading : false,
                forceUpdate: this.$forceUpdate.bind(this),
            }
        },
        methods: {
            sendMedicaments: function () {
                const medicaments = Array.from(this.selectedMedicaments.values());
                console.log(medicaments);
                this.isLoading = true;
                fetch('/api/aggregate', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({medicaments: medicaments, settings: this.settings})
                })
                    .then(e => e.json())
                    .then(e => {
                        this.isLoading = false;
                        this.$router.push({name: "result", params: {aggregateResult: e, selectedMedicaments: this.selectedMedicaments, settings: this.settings}})
                    });
            },
            deleteMedicament : function (key) {
                this.selectedMedicaments.delete(key);
                this.$forceUpdate();
            },
        },
    }
    
</script>

<style scoped>
    .main {
        margin: auto; 
        width: 600px;
        height: 400px;
        display: grid;
        grid-template-rows: 50px 50px 300px;
        grid-template-columns: 1fr;
        grid-row-gap: 5px;
    }
    
    .search-wrap {
        grid-row: 1/2;
    }
    
    .controls {
        grid-row: 2/3;
        display: grid;
        grid-template-columns: 5fr 1fr;
    }
    
    .search-btn {
        grid-column: 2/3;
        width: 100%;
        height: 100%;
    }
    
    .selected-medicament-list-wrap {
        display: grid;
        grid-template-rows: repeat(12, 1fr);
        grid-row-gap: 5px;
    }
</style>