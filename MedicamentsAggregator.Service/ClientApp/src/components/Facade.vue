<template>
    <main class="facade">
        <section class="search-wrap">
            <Search v-bind:commonData="commonData"/>
        </section>
        
        <div class="controls">
            <div class="settings"></div>
            <button class="search-btn" v-on:click="sendMedicaments">Искать</button>
        </div>
        

        <div class="selected-medicament-list-wrap">
            <SelectedMedicament
                    v-for="[key,value] in commonData.selectedMedicaments"
                    v-bind:key="key"
                    :medicament="value"
            />
        </div>
    </main>
    
</template>

<script>
    import Search from "./Search";
    import SelectedMedicament from "./SelectedMedicament";
    export default {
        name: "Facade",
        components: {SelectedMedicament, Search},
        data: function () {
            return {
                commonData: {
                    forceUpdate: this.$forceUpdate.bind(this),
                    selectedMedicaments: new Map()
                }
            }
        },
        methods: {
            sendMedicaments: function () {
                console.log(Array.from(this.commonData.selectedMedicaments.values()));
            }
        }
    }
    
</script>

<style scoped>
    .facade {
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
        grid-template-columns: repeat(6, 1fr);
    }
    .settings {
        grid-column: 1/6;
    }
    .search-btn {
        grid-column: 6/7;
        width: 100%;
        height: 100%;
    }
    
    .selected-medicament-list-wrap {
        display: grid;
        grid-template-rows: repeat(12, 1fr);
        grid-row-gap: 5px;
    }
</style>