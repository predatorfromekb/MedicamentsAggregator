<template>
    <section class="selected-medicament">
        <span class="selected-medicament__title">{{medicament.title}}</span>
        <div class="selected-medicament__button-list">
            <input type="number" class="selected-medicament__count" v-model="countValue" v-on:keyup="onkeyup"/>
            <button v-on:click="deleteMedicament">❌</button>
        </div>
    </section>
</template>

<script>
    export default {
        name: "SelectedMedicament",
        props: {
            deleteMedicament: Function,
            medicament: Object
        },
        data : function () {
            return {
                countValue: this.medicament.count
            }
        },
        methods: {
            onkeyup: function() {
                this.medicament.count = parseInt(this.countValue);
            },
        }
    }
</script>

<style scoped>
    .selected-medicament {
        display: grid;
        grid-template-columns: repeat(6, 1fr);
        
        white-space: nowrap;
        vertical-align: middle;
        
    }
    .selected-medicament__title {
        text-overflow: ellipsis;
        overflow: hidden;
        grid-column: span 5;
        padding-left: 5px;
    }

    .selected-medicament__count {
        text-align: center;
        grid-column: span 1;
        width: 100%;
    }

    .selected-medicament__button-list {
        grid-column: span 1;
        display: grid;
        grid-template-columns: repeat(2, 1fr);
        grid-column-gap: 5px;
    }
    
</style>